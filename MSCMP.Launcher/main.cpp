#include <windows.h>
#include <iostream>
#include <filesystem>
#include <fstream>
#include <string>
#include "steam_api.h"

/// <summary>
/// Displays an error message box with the specified text.
/// </summary>
/// <param name="text">- The message text to display in the message box.</param>
int ShowMessageBox(LPCWSTR text) {
    MessageBoxW(nullptr, text, L"MSCMP Launcher", MB_ICONERROR);
    return -1;
}

/// <summary>
/// Retrieves the error message string corresponding to a Windows error code.
/// </summary>
/// <param name="err">- The Windows error code for which to retrieve the message. Defaults to the result of GetLastError().</param>
std::wstring GetFormattedLastErrorMessage(DWORD err = GetLastError()) {
	// Get the string message for the last error code.
    wchar_t* pBuffer = nullptr;
    FormatMessageW(FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS,
        nullptr, err, 0, reinterpret_cast<LPWSTR>(&pBuffer), 0, nullptr );

	// If the message buffer is null, return a default message.
    std::wstring msg = pBuffer ? pBuffer : L"Unknown error code!";
    LocalFree(pBuffer);
    return msg;
}

/// <summary>
/// Converts a UTF-8 string to a wide string.
/// </summary>
/// <param name="s">- Pointer to a null-terminated UTF-8 string.</param>
std::wstring ToWide(const char* s) {
    int wlen = MultiByteToWideChar(CP_UTF8, 0, s, -1, nullptr, 0);
    std::wstring wstr(wlen, 0);
    MultiByteToWideChar(CP_UTF8, 0, s, -1, &wstr[0], wlen);
    return wstr;
}

// Entrypoint.
int WINAPI wWinMain(HINSTANCE, HINSTANCE, PWSTR, int) {
    // Fetch the base directory.
    wchar_t modulePath[MAX_PATH];
    GetModuleFileNameW(nullptr, modulePath, MAX_PATH);

	// GetModuleFileNameW returns the full path to the module.
    // We need to fetch the parent directory (also convert it to fs::path).
    auto baseDirectory = std::filesystem::path(modulePath).parent_path();
    auto clientDll = baseDirectory / L"MSCMP.dll";
    auto injectorDll = baseDirectory / L"MSCMP.MonoInjector.dll";

    // Check if the client file exists.
    if (!std::filesystem::exists(clientDll)) {
		return ShowMessageBox(L"MSCMP.dll is missing!");
    }

	// And now check if the injector file exists.
    if (!std::filesystem::exists(injectorDll)) {
        return ShowMessageBox(L"MSCMP.MonoInjector.dll is missing!");
    }

	// Check if the steam_appid.txt file exists, if not, create it.
	// This is needed to initialize the Steam interfaces.
    auto appidFile = baseDirectory / L"steam_appid.txt";
    if (!std::filesystem::exists(appidFile)) {
        std::ofstream of(appidFile.string());
        if (!of) {
            return ShowMessageBox(L"Failed to create the steam_appid.txt\n\nYou must create it manually with \"516750\" as the contents of it");
        }
        
        // Write the app ID for MSC.
        of << 516750;
    }

    // Initialize Steam interfaces.
    SteamErrMsg steamErr;
    if (SteamAPI_InitEx(&steamErr) != k_ESteamAPIInitResult_OK) {
        return ShowMessageBox((L"SteamAPI_InitEx failed: " + ToWide(steamErr)).c_str());
    }

    // Check if the game is installed.
    if (!SteamApps()->BIsAppInstalled(516750)) {
        SteamAPI_Shutdown();
		return ShowMessageBox(L"My Summer Car is not installed on your local machine");
    }

    // Get the install directory for the game.
    // This can still return a valid path (which would be the default location).
    char installPath[MAX_PATH];
    if (SteamApps()->GetAppInstallDir(516750, installPath, MAX_PATH) == 0) {
        SteamAPI_Shutdown();
        return ShowMessageBox(L"Failed to get the installation directory for My Summer Car");
    }

    // Convert the install path to a usable fs::path.
    auto gameDir = std::filesystem::path(installPath);

	// Check if the game executable exists. This will verify that the path returned by SteamApps::GetAppInstallDir() is valid.
    auto exePath = gameDir / L"mysummercar.exe";
    if (!std::filesystem::exists(exePath)) {
        SteamAPI_Shutdown();
        return ShowMessageBox(L"Install directory does not contain the game executable. Verify files via Steam to resolve this");
    }

	// The game process will inherit this environment variable.
    SetEnvironmentVariableW(L"MSCMP_CLIENT_PATH", clientDll.c_str());

    // Launch the game process.
    STARTUPINFOW si{ sizeof(si) };
    PROCESS_INFORMATION pi{};
    if (!CreateProcessW(exePath.c_str(), nullptr, nullptr, nullptr, FALSE, NORMAL_PRIORITY_CLASS, nullptr, gameDir.c_str(), &si, &pi)) {
        SteamAPI_Shutdown();
        return ShowMessageBox((L"Failed to create the game process: " + GetFormattedLastErrorMessage()).c_str());
    }

    // Wait for the GUI to show up.
    WaitForInputIdle(pi.hProcess, INFINITE);

	// Reserve memory in the target process for the injector DLL path.
    size_t len = (injectorDll.string().size() + 1) * sizeof(wchar_t);
    LPVOID remoteMem = VirtualAllocEx(pi.hProcess, nullptr, (injectorDll.string().size() + 1) * sizeof(wchar_t), MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);
    if (!remoteMem) {
        TerminateProcess(pi.hProcess, 1);
        SteamAPI_Shutdown();
        return ShowMessageBox((L"Failed to reserve memory: " + GetFormattedLastErrorMessage()).c_str());
    }

	// Write the injector DLL path to the reserved memory.
    if (!WriteProcessMemory(pi.hProcess, remoteMem, injectorDll.c_str(), len, nullptr)) {
        VirtualFreeEx(pi.hProcess, remoteMem, 0, MEM_RELEASE);
        TerminateProcess(pi.hProcess, 1);
        SteamAPI_Shutdown();
        return ShowMessageBox((L"Failed to write data to the reserved memory region: " + GetFormattedLastErrorMessage()).c_str());
    }

	// Get the address of LoadLibraryW in kernel32.dll.
    auto loadLib = reinterpret_cast<LPTHREAD_START_ROUTINE>(GetProcAddress(GetModuleHandleW(L"kernel32.dll"), "LoadLibraryW"));
    if (!loadLib) {
        std::wcerr << L"GetProcAddress failed: " << GetFormattedLastErrorMessage() << std::endl;
        VirtualFreeEx(pi.hProcess, remoteMem, 0, MEM_RELEASE);
        TerminateProcess(pi.hProcess, 1);
        SteamAPI_Shutdown();
        return ShowMessageBox((L"Failed to get the address of LoadLibrary: " + GetFormattedLastErrorMessage()).c_str());
    }

	// Create a thread in the target process to load the injector DLL.
    HANDLE hThread = CreateRemoteThread(pi.hProcess, nullptr, 0, loadLib, remoteMem, 0, nullptr);
    if (!hThread) {
        VirtualFreeEx(pi.hProcess, remoteMem, 0, MEM_RELEASE);
        TerminateProcess(pi.hProcess, 1);
        SteamAPI_Shutdown();
        return ShowMessageBox((L"Failed to create a new remote thread: " + GetFormattedLastErrorMessage()).c_str());
    }

    // Wait for the remote thread to finish.
    WaitForSingleObject(hThread, INFINITE);
    CloseHandle(hThread);

    // Cleanup
    VirtualFreeEx(pi.hProcess, remoteMem, 0, MEM_RELEASE);
    CloseHandle(pi.hProcess);
    CloseHandle(pi.hThread);
    SteamAPI_Shutdown();
    return 0;
}
