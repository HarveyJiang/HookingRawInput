/*
	This project serves as a simple demonstration for the article "Combining Raw Input and keyboard Hook to selectively block input from multiple keyboards",
	which you should be able to find in this project folder (HookingRawInput.html), or on the CodeProject website (http://www.codeproject.com/).
	The project is based on the idea shown to me by Petr Medek (http://www.hidmacros.eu/), and is published with his permission, huge thanks to Petr!
	The source code is licensed under The Code Project Open License (CPOL), feel free to adapt it.
	Vít Blecha (sethest@gmail.com), 2014
*/

#ifdef HOOKINGRAWINPUTDEMODLL_EXPORTS
	#define HOOKINGRAWINPUTDEMODLL_API __declspec(dllexport)
#else
	#define HOOKINGRAWINPUTDEMODLL_API __declspec(dllimport)
#endif

HOOKINGRAWINPUTDEMODLL_API BOOL InstallHook (HWND hwndParent);

HOOKINGRAWINPUTDEMODLL_API BOOL UninstallHook ();
