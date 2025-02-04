#include <filesystem>
#include <fstream>
#include <iostream>
#include <Windows.h>
#include "steam_api.h"

#pragma warning(disable : 6387)

static void LogError(const std::wstring& message) {
	HANDLE console = GetStdHandle(STD_OUTPUT_HANDLE);
	SetConsoleTextAttribute(console, 71);
	std::wcerr << message << std::endl;
	SetConsoleTextAttribute(console, 7);
}

int wmain(int argc, wchar_t* argv[])
{
	// Get current working directory first.
	wchar_t path[MAX_PATH];
	GetModuleFileNameW(nullptr, path, MAX_PATH);
	std::filesystem::path workingDir = std::filesystem::path(path).parent_path();
	
	// File integrity.
	std::filesystem::path clientPath = workingDir / L"MSCMP.Client.dll";
	std::filesystem::path injectorPath = workingDir / L"MSCMP.Injector.dll";
	if (!std::filesystem::exists(clientPath) || !std::filesystem::exists(injectorPath)) {
		LogError(L"Critical mod files are missing! Ensure that MSCMP.Client.dll and MSCMP.Injector.dll are present");
		system("pause");
		return -1;
	}

	// Create a steam_appid.txt file if it's missing.
	// This is needed for SteamAPI.
	std::filesystem::path appIdPath = workingDir / L"steam_appid.txt";
	if (!std::filesystem::exists(appIdPath)) {
		std::wofstream appId(appIdPath);
		appId << L"516750";
	}

	// Initialize.
	if (!SteamAPI_Init()) {
		LogError(L"SteamAPI_Init() failed! Ensure Steam is running, you own MSC, and Steam is online.");
		system("pause");
		return -1;
	}

	// Check if the game is installed.
	if (!SteamApps()->BIsAppInstalled(516750)) {
		SteamAPI_Shutdown();
		LogError(L"My Summer Car is not installed on your local machine");
		system("pause");
		return -1;
	}

	// Get the install folder.
	std::vector<char> installFolder(MAX_PATH);
	uint32 bufferSize = SteamApps()->GetAppInstallDir(516750, installFolder.data(), MAX_PATH);
	if (bufferSize == 0 || installFolder[0] == '\0') {
		SteamAPI_Shutdown();
		LogError(L"Buffer is empty - Failed to retrieve MSC directory");
		system("pause");
		return -1;
	}

	// Store the real install path.
	std::wstring gameInstallFolder(bufferSize, L'\0');
	int wideLength = MultiByteToWideChar(CP_UTF8, 0, installFolder.data(), -1, &gameInstallFolder[0], bufferSize);
	gameInstallFolder.resize(wideLength - 1); // Resize the string to exclude the null terminator

	// Verify the game executable exists.
	std::filesystem::path exePath = gameInstallFolder + L"\\mysummercar.exe";
	if (!std::filesystem::exists(exePath)) {
		SteamAPI_Shutdown();
		LogError(L"Game executable is missing from the install directory! Verify integrity of game files");
		system("pause");
		return -1;
	}

	STARTUPINFOW si = { sizeof(si) };
	PROCESS_INFORMATION pi = { 0 };

	// Construct the command line.
	std::wstring cmdLine = argv[0];
	for (int i = 1; i < argc; ++i) {
		cmdLine += L" ";
		cmdLine += argv[i];
	}

	// TODO: Temporary cmdline fix because I have poo poo PC.
	// Also too lazy to launch a batch file.
	if (SteamUser()->GetSteamID() == CSteamID(static_cast<uint64_t>(76561198056341327))) {
		cmdLine += L" ";
		cmdLine += L"-force-d3d9";
	}

	// Set the environment variable. The game will automatically inherit this.
	SetEnvironmentVariableW(L"MSCMP_CLIENT_PATH", clientPath.c_str());

	// Start the game process
	if (!CreateProcessW(exePath.c_str(), cmdLine.data(), nullptr, nullptr, FALSE, NORMAL_PRIORITY_CLASS, nullptr, gameInstallFolder.c_str(), &si, &pi)) {
		LogError(L"Failed to start the game process. Error code: " + std::to_wstring(GetLastError()));
		system("pause");
		return -1;
	}

	// Wait for the configuration window to show up.
	WaitForInputIdle(pi.hProcess, INFINITE);

	// Lambda function for cleanup.
	auto CleanupProcess = [&pi](const std::wstring& message) {
		TerminateProcess(pi.hProcess, 1);
		CloseHandle(pi.hProcess);
		CloseHandle(pi.hThread);
		SteamAPI_Shutdown();
		LogError(message);
		system("pause");
		};

	// Allocate memory for DLL injection
	LPVOID baseAddress = VirtualAllocEx(pi.hProcess, nullptr, injectorPath.wstring().length() * sizeof(wchar_t) + 1, MEM_COMMIT, PAGE_READWRITE);
	if (!baseAddress) {
		CleanupProcess(L"Failed to reserve virtual memory! Return value: " + std::to_wstring(GetLastError()));
		return -1;
	}

	// Write injector path to process memory
	if (!WriteProcessMemory(pi.hProcess, baseAddress, injectorPath.c_str(), (injectorPath.wstring().length() + 1) * sizeof(wchar_t), nullptr)) {
		VirtualFreeEx(pi.hProcess, baseAddress, 0, MEM_RELEASE);
		CleanupProcess(L"Failed to write injector path to the reserved memory region! Return value: " + std::to_wstring(GetLastError()));
		return -1;
	}

	// Get LoadLibraryW address and create remote thread
	HMODULE kernel32 = GetModuleHandleW(L"kernel32.dll");
	FARPROC loadLibraryAddress = GetProcAddress(kernel32, "LoadLibraryW");
	if (!loadLibraryAddress) {
		VirtualFreeEx(pi.hProcess, baseAddress, 0, MEM_RELEASE);
		CleanupProcess(L"Failed to get address of LoadLibraryW from kernel32.dll! Return value: " + std::to_wstring(GetLastError()));
		return -1;
	}

	HANDLE remoteThread = CreateRemoteThread(pi.hProcess, nullptr, 0, (LPTHREAD_START_ROUTINE)loadLibraryAddress, baseAddress, 0, nullptr);
	if (!remoteThread) {
		VirtualFreeEx(pi.hProcess, baseAddress, 0, MEM_RELEASE);
		CleanupProcess(L"Failed to create remote thread! Return value: " + std::to_wstring(GetLastError()));
		return -1;
	}

	// Wait for injector to finish and close up.
	WaitForSingleObject(remoteThread, INFINITE);
	CloseHandle(remoteThread);
	VirtualFreeEx(pi.hProcess, baseAddress, 0, MEM_RELEASE);
	CloseHandle(pi.hProcess);
	CloseHandle(pi.hThread);
	return 0;
}