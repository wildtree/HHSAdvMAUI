#define MyAppName    "ハイハイスクールアドベンチャー(.NET MAUI)"
#define MyAppExeName "HHSAdvMAUI.exe"
#define MyAppVersion GetFileVersion("..\bin\Release\net9.0-windows10.0.19041.0\win10-x64\publish\HHSAdvMAUI.exe")

[Setup]
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher=WildTreeJP
DefaultDirName={commonpf64}\HHSAdvMAUI
DefaultGroupName={#MyAppName}
UninstallDisplayIcon={app}\{#MyAppExeName}
OutputBaseFilename=hhsadvmaui_setup_{#MyAppVersion}
Compression=zip
SolidCompression=yes
ArchitecturesInstallIn64BitMode=x64compatible

[Files]
Source: "..\bin\Release\net9.0-windows10.0.19041.0\win10-x64\publish\*"; DestDir: "{app}"; Flags: recursesubdirs

[Languages]
Name: "japanese"; MessagesFile: "compiler:Languages\Japanese.isl"
Name: "english"; MessagesFile: "compiler:Default.isl"


[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,ハイハイスクールアドベンチャー(.NET MAUI)}"; Flags: nowait postinstall skipifsilent
