#define WIN32_LEAN_AND_MEAN
#include <windows.h>
#include <iostream>
#include <filesystem>
#include <psapi.h>

typedef VOID MonoDomain;
typedef VOID MonoThread;
typedef VOID MonoImage;
typedef VOID MonoAssembly;
typedef VOID MonoClass;
typedef VOID MonoMethod;
typedef VOID MonoObject;

typedef MonoDomain* (__cdecl* mono_get_root_domain_t)();
typedef MonoThread* (__cdecl* mono_thread_attach_t)(MonoDomain* mDomain);
typedef MonoImage* (__cdecl* mono_assembly_get_image_t)(MonoAssembly* assembly);
typedef MonoAssembly* (__cdecl* mono_domain_assembly_open_t)(MonoDomain* mDomain, const char* filepath);
typedef MonoClass* (__cdecl* mono_class_from_name_t)(MonoImage* image, const char* name_space, const char* name);
typedef MonoMethod* (__cdecl* mono_class_get_method_from_name_t)(MonoClass* mclass, const char* name, int param_count);
typedef MonoObject* (__cdecl* mono_runtime_invoke_t)(MonoMethod* method, void* obj, void** params, MonoObject** exc);

static bool WriteErrorMessage(const std::string message) {
    MessageBoxA(NULL, message.c_str(), "MSCMP", MB_ICONERROR);
    ExitProcess(0);
    return FALSE;
}

static FARPROC GetMonoFunctionAddress(HMODULE module, const char* procName) {
    FARPROC proc_address = GetProcAddress(module, procName);
    if (!proc_address) {
        WriteErrorMessage("Failed to get address for mono function: " + std::string(procName) + "!");
    }

    return proc_address;
}

BOOL APIENTRY DllMain(HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved)
{
    if (ul_reason_for_call == DLL_PROCESS_ATTACH) {
        DisableThreadLibraryCalls(hModule);

        char buf[256];
        GetEnvironmentVariableA("MSCMP_CLIENT_PATH", buf, sizeof(buf));

        HMODULE mono = GetModuleHandleA("mono.dll");
        if (!mono) {
            WriteErrorMessage("Failed to get mono module!");
            return FALSE;
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
            WriteErrorMessage("Failed to open client assembly into the Mono domain!");
            return FALSE;
        }

        MonoImage* clientImage = mono_assembly_get_image(clientAsm);
        if (!clientImage) {
            WriteErrorMessage("Failed to open client assembly image!");
            return FALSE;
        }

        MonoClass* entrypointClass = mono_class_from_name(clientImage, "MSCMP", "Client");
        if (!entrypointClass) {
            WriteErrorMessage("Failed to get MSCMP::Client class entrypoint!");
            return FALSE;
        }

        MonoMethod* entrypointMethod = mono_class_get_method_from_name(entrypointClass, "Start", 0);
        if (!entrypointMethod) {
            WriteErrorMessage("Failed to get Client::Start() entrypoint method!");
            return FALSE;
        }

        mono_runtime_invoke(entrypointMethod, nullptr, nullptr, nullptr);
    }

    return TRUE;
}