#define WIN32_LEAN_AND_MEAN
#include <windows.h>
#include <iostream>
#include <optional>

// Mono type definitions
typedef void MonoDomain;
typedef void MonoThread;
typedef void MonoImage;
typedef void MonoAssembly;
typedef void MonoClass;
typedef void MonoMethod;
typedef void MonoObject;

// Function pointer typedefs for Mono
typedef MonoDomain* (__fastcall* mono_get_root_domain_t)();
typedef MonoThread* (__fastcall* mono_thread_attach_t)(MonoDomain* mDomain);
typedef MonoImage* (__fastcall* mono_assembly_get_image_t)(MonoAssembly* assembly);
typedef MonoAssembly* (__fastcall* mono_domain_assembly_open_t)(MonoDomain* mDomain, const char* filepath);
typedef MonoClass* (__fastcall* mono_class_from_name_t)(MonoImage* image, const char* name_space, const char* name);
typedef MonoMethod* (__fastcall* mono_class_get_method_from_name_t)(MonoClass* mclass, const char* name, int param_count);
typedef MonoObject* (__fastcall* mono_runtime_invoke_t)(MonoMethod* method, void* obj, void** params, MonoObject** exc);

/**
 * @brief Displays an error message in a message box.
 * @param message - The error message to display.
 */
static bool WriteErrorMessage(const std::string & message) {
    MessageBoxA(nullptr, message.c_str(), "MSCMP", MB_ICONERROR);
    ExitProcess(0);
    return FALSE;
}

/**
 * @brief Retrieves a function address from the Mono module.
 * @param module - Mono module.
 * @param procName - The name of the function to retrieve.
 * @return The function address, or nullptr if not found.
 */
static FARPROC GetMonoFunctionAddress(HMODULE module, const char* procName) {
    FARPROC proc_address = GetProcAddress(module, procName);
    if (!proc_address) {
        WriteErrorMessage("Failed to get address for mono function: " + std::string(procName) + "!");
        return nullptr;
    }
    return proc_address;
}

// Entrypoint.
BOOL APIENTRY DllMain(HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved)
{
    if (ul_reason_for_call == DLL_PROCESS_ATTACH) {
        DisableThreadLibraryCalls(hModule);

        // This is set by the launcher.
        char buf[256];
        if (GetEnvironmentVariableA("MSCMP_CLIENT_PATH", buf, sizeof(buf)) == 0) {
            return WriteErrorMessage("Failed to get the MSCMP_CLIENT_PATH environment variable!");
        }

        HMODULE mono;
        if (!GetModuleHandleExA(GET_MODULE_HANDLE_EX_FLAG_PIN, "mono.dll", &mono)) {
            return WriteErrorMessage("Failed to get mono module!");
        }

        mono_get_root_domain_t mono_get_root_domain = (mono_get_root_domain_t)GetMonoFunctionAddress(mono, "mono_get_root_domain");
        mono_thread_attach_t mono_thread_attach = (mono_thread_attach_t)GetMonoFunctionAddress(mono, "mono_thread_attach");
        mono_class_from_name_t mono_class_from_name = (mono_class_from_name_t)GetMonoFunctionAddress(mono, "mono_class_from_name");
        mono_class_get_method_from_name_t mono_class_get_method_from_name = (mono_class_get_method_from_name_t)GetMonoFunctionAddress(mono, "mono_class_get_method_from_name");
        mono_runtime_invoke_t mono_runtime_invoke = (mono_runtime_invoke_t)GetMonoFunctionAddress(mono, "mono_runtime_invoke");
        mono_domain_assembly_open_t mono_domain_assembly_open = (mono_domain_assembly_open_t)GetMonoFunctionAddress(mono, "mono_domain_assembly_open");
        mono_assembly_get_image_t mono_assembly_get_image = (mono_assembly_get_image_t)GetMonoFunctionAddress(mono, "mono_assembly_get_image");

        MonoDomain* rootDomain = mono_get_root_domain();
        if (!rootDomain) {
            return WriteErrorMessage("Failed to get root domain!");
        }

        mono_thread_attach(rootDomain);

        MonoAssembly* clientAsm = mono_domain_assembly_open(rootDomain, buf);
        if (!clientAsm) {
            return WriteErrorMessage("Failed to open client assembly into the Mono domain!");
        }

        MonoImage* clientImage = mono_assembly_get_image(clientAsm);
        if (!clientImage) {
            return WriteErrorMessage("Failed to open client assembly image!");
        }

        MonoClass* entrypointClass = mono_class_from_name(clientImage, "MSCMP", "Client");
        if (!entrypointClass) {
            return WriteErrorMessage("Failed to get MSCMP::Client class entrypoint!");
        }

        MonoMethod* entrypointMethod = mono_class_get_method_from_name(entrypointClass, "Start", 0);
        if (!entrypointMethod) {
            return WriteErrorMessage("Failed to get Client::Start() entrypoint method!");
        }

        mono_runtime_invoke(entrypointMethod, nullptr, nullptr, nullptr);
    }

    return TRUE;
}