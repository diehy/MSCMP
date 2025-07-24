#include <Windows.h>

typedef void MonoDomain;
typedef void MonoThread;
typedef void MonoImage;
typedef void MonoAssembly;
typedef void MonoClass;
typedef void MonoMethod;
typedef void MonoObject;
typedef void MonoString;

typedef MonoDomain* (__stdcall* mono_get_root_domain_t)(void);
typedef MonoThread* (__stdcall* mono_thread_attach_t)(MonoDomain*);
typedef MonoImage* (__stdcall* mono_assembly_get_image_t)(MonoAssembly*);
typedef MonoAssembly* (__stdcall* mono_domain_assembly_open_t)(MonoDomain*, const char*);
typedef MonoClass* (__stdcall* mono_class_from_name_t)(MonoImage*, const char*, const char*);
typedef MonoMethod* (__stdcall* mono_class_get_method_from_name_t)(MonoClass*, const char*, int);
typedef MonoObject* (__stdcall* mono_runtime_invoke_t)(MonoMethod*, void*, void**, MonoObject**);
typedef MonoClass* (__stdcall* mono_object_get_class_t)(MonoObject*);
typedef const char* (__stdcall* mono_string_to_utf8_t)(MonoString*);

// This function never returns, it shows a message box and exits the process.
__declspec(noreturn) void Exit(const char* msg) {
    MessageBoxA(NULL, msg, "My Summer Car - Multiplayer", MB_ICONERROR);
    ExitProcess(1);
}

// Worker thread for invoking Mono methods.
// Seems to fix an issue with the mod not working in Sandboxie.
DWORD WINAPI InvokeMono(LPVOID lpParameter) {
    // Get the absolute path to the client assembly.
    char clientPath[MAX_PATH];
    if (!GetEnvironmentVariableA("MSCMP_CLIENT_PATH", clientPath, MAX_PATH))
        Exit("No environment variable was set to indicate the client location!");

    // Get the Mono module from the game process.
    HMODULE mono = GetModuleHandleA("mono.dll");
    if (!mono)
        Exit("The game process does not contain mono.dll!");

    // Helper macro for loading Mono functions.
    #define LOAD_MONO_FUNC(var, type, name) \
    var = (type)GetProcAddress(mono, name); \
    if (!(var)) Exit("Failed to resolve Mono function: " name);

    // Mono API function pointers.
    mono_get_root_domain_t mono_get_root_domain = NULL;
    LOAD_MONO_FUNC(mono_get_root_domain, mono_get_root_domain_t, "mono_get_root_domain");

    mono_thread_attach_t mono_thread_attach = NULL;
    LOAD_MONO_FUNC(mono_thread_attach, mono_thread_attach_t, "mono_thread_attach");

    mono_domain_assembly_open_t mono_domain_assembly_open = NULL;
    LOAD_MONO_FUNC(mono_domain_assembly_open, mono_domain_assembly_open_t, "mono_domain_assembly_open");

    mono_assembly_get_image_t mono_assembly_get_image = NULL;
    LOAD_MONO_FUNC(mono_assembly_get_image, mono_assembly_get_image_t, "mono_assembly_get_image");

    mono_class_from_name_t mono_class_from_name = NULL;
    LOAD_MONO_FUNC(mono_class_from_name, mono_class_from_name_t, "mono_class_from_name");

    mono_class_get_method_from_name_t mono_class_get_method_from_name = NULL;
    LOAD_MONO_FUNC(mono_class_get_method_from_name, mono_class_get_method_from_name_t, "mono_class_get_method_from_name");

    mono_runtime_invoke_t mono_runtime_invoke = NULL;
    LOAD_MONO_FUNC(mono_runtime_invoke, mono_runtime_invoke_t, "mono_runtime_invoke");

    mono_object_get_class_t mono_object_get_class = NULL;
    LOAD_MONO_FUNC(mono_object_get_class, mono_object_get_class_t, "mono_object_get_class");

    mono_string_to_utf8_t mono_string_to_utf8 = NULL;
    LOAD_MONO_FUNC(mono_string_to_utf8, mono_string_to_utf8_t, "mono_string_to_utf8");

    // Get the root domain.
    MonoDomain* domain = mono_get_root_domain();
    if (!domain) Exit("Failed to obtain the Mono root domain!");

    // Attach the current thread to the Mono domain.
    mono_thread_attach(domain);

    // Open the target client assembly
    MonoAssembly* assembly = mono_domain_assembly_open(domain, clientPath);
    if (!assembly) Exit("Failed to open the client assembly!");

    // Get the assembly image
    MonoImage* image = mono_assembly_get_image(assembly);
    if (!image) Exit("Failed to get the assembly image!");

    // Find the entrypoint class
    MonoClass* klass = mono_class_from_name(image, "MSCMP", "Bootstrap");
    if (!klass) Exit("Failed to obtain the client entrypoint (TYPE)!");

    // Find the entrypoint method.
    MonoMethod* method = mono_class_get_method_from_name(klass, "Initialize", 0);
    if (!method) Exit("Failed to obtain the client entrypoint (FUNCTION)!");

    // Call it.
    MonoObject* exc = NULL;
    mono_runtime_invoke(method, NULL, NULL, &exc);

	// Handle any exceptions that occurred during bootstrapping.
    if (exc) {
        MonoMethod* toString = mono_class_get_method_from_name(mono_object_get_class(exc), "ToString", 0);
        if (toString) {
            MonoObject* strExc = NULL;
            MonoString* str = (MonoString*)mono_runtime_invoke(toString, exc, NULL, &strExc);
            if (!strExc && str) {
                Exit(mono_string_to_utf8(str));
            }
        }

        Exit("Exception occurred during bootstrap, but the exception message cannot be displayed!");
    }

    return 0;
}

// Entrypoint.
BOOL WINAPI DllMain(HMODULE hModule, DWORD reason, LPVOID reserved)
{
    if (reason == DLL_PROCESS_ATTACH) {
        DisableThreadLibraryCalls(hModule);
        CreateThread(NULL, 0, InvokeMono, NULL, 0, NULL);
    }

    return TRUE;
}