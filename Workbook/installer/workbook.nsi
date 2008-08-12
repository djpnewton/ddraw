!include "registerExtension.nsh"

;--------------------------------

; helper defines

!define PRODUCT_NAME "2Touch Workbook"
!define PRODUCT_VERSION "1.0"
!define PRODUCT_WEB_SITE "http://www.twotouch.com"
!define PRODUCT_DIR_REGKEY "Software\Microsoft\Windows\CurrentVersion\App Paths\Workbook.exe"
!define PRODUCT_UNINST_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}"
!define PRODUCT_UNINST_ROOT_KEY "HKLM"

!define FILE_ASSO_EXT ".wbook"
!define FILE_ASSO_NAME "${PRODUCT_NAME} File"

;--------------------------------

; Installer Attributes

Name "${PRODUCT_NAME} ${PRODUCT_VERSION}"
OutFile "WorkbookSetup.exe"
InstallDir "$PROGRAMFILES\2Touch Workbook"
InstallDirRegKey HKLM "${PRODUCT_DIR_REGKEY}" ""
ShowInstDetails show
ShowUnInstDetails show
SetCompressor lzma

;--------------------------------

; MUI 1.67 compatible ------
!include "MUI.nsh"

; MUI Settings
!define MUI_ABORTWARNING
!define MUI_ICON "${NSISDIR}\Contrib\Graphics\Icons\modern-install.ico"
!define MUI_UNICON "${NSISDIR}\Contrib\Graphics\Icons\modern-uninstall.ico"

; Welcome page
!insertmacro MUI_PAGE_WELCOME
; License page
!insertmacro MUI_PAGE_LICENSE "license.txt"
; Components page
!insertmacro MUI_PAGE_COMPONENTS
; Directory page
!insertmacro MUI_PAGE_DIRECTORY
; Instfiles page
!insertmacro MUI_PAGE_INSTFILES
; Finish page
!define MUI_FINISHPAGE_RUN "$INSTDIR\Workbook.exe"
!insertmacro MUI_PAGE_FINISH

; Uninstaller pages
!insertmacro MUI_UNPAGE_INSTFILES

; Language files
!insertmacro MUI_LANGUAGE "English"

; Reserve files
!insertmacro MUI_RESERVEFILE_INSTALLOPTIONS

; MUI end ------

;--------------------------------

; The stuff to install

Section "Install Program" SecMain
  SectionIn RO
  SetOutPath "$INSTDIR"
  SetOverwrite ifnewer
  File "..\bin\Release\Workbook.exe"
  CreateDirectory "$SMPROGRAMS\2Touch Workbook"
  CreateShortCut "$SMPROGRAMS\2Touch Workbook\2Touch Workbook.lnk" "$INSTDIR\Workbook.exe"
  CreateShortCut "$DESKTOP\2Touch Workbook.lnk" "$INSTDIR\Workbook.exe"
  File "..\bin\Release\*.dll"
SectionEnd

Section -AdditionalIcons
  WriteIniStr "$INSTDIR\${PRODUCT_NAME}.url" "InternetShortcut" "URL" "${PRODUCT_WEB_SITE}"
  CreateShortCut "$SMPROGRAMS\2Touch Workbook\Website.lnk" "$INSTDIR\${PRODUCT_NAME}.url"
  CreateShortCut "$SMPROGRAMS\2Touch Workbook\Uninstall.lnk" "$INSTDIR\uninst.exe"
SectionEnd

Section "Register File Extension" SecRegExt
  ${registerExtension} "$INSTDIR\Workbook.exe" "${FILE_ASSO_EXT}" "${FILE_ASSO_NAME}"
SectionEnd

Section -Post
  WriteUninstaller "$INSTDIR\uninst.exe"
  WriteRegStr HKLM "${PRODUCT_DIR_REGKEY}" "" "$INSTDIR\Workbook.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayName" "$(^Name)"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "UninstallString" "$INSTDIR\uninst.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayIcon" "$INSTDIR\Workbook.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayVersion" "${PRODUCT_VERSION}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "URLInfoAbout" "${PRODUCT_WEB_SITE}"
SectionEnd

Function un.onUninstSuccess
  HideWindow
  MessageBox MB_ICONINFORMATION|MB_OK "$(^Name) was successfully removed from your computer."
FunctionEnd

Function un.onInit
  MessageBox MB_ICONQUESTION|MB_YESNO|MB_DEFBUTTON2 "Are you sure you want to completely remove $(^Name) and all of its components?" IDYES +2
  Abort
FunctionEnd

Section Uninstall
  ${unregisterExtension} "${FILE_ASSO_EXT}" "${FILE_ASSO_NAME}"

  Delete "$INSTDIR\${PRODUCT_NAME}.url"
  Delete "$INSTDIR\uninst.exe"
  Delete "$INSTDIR\*.dll"
  Delete "$INSTDIR\Workbook.exe"
  Delete "$INSTDIR\*.ini" # remove ini files

  Delete "$SMPROGRAMS\2Touch Workbook\Uninstall.lnk"
  Delete "$SMPROGRAMS\2Touch Workbook\Website.lnk"
  Delete "$DESKTOP\2Touch Workbook.lnk"
  Delete "$SMPROGRAMS\2Touch Workbook\2Touch Workbook.lnk"

  RMDir "$SMPROGRAMS\2Touch Workbook"
  RMDir "$INSTDIR"

  DeleteRegKey ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}"
  DeleteRegKey HKLM "${PRODUCT_DIR_REGKEY}"
  SetAutoClose true
SectionEnd

;--------------------------------

; Descriptions

  ;Language strings
  LangString DESC_SecMain ${LANG_ENGLISH} "Intall ${PRODUCT_NAME}."
  LangString DESC_SecRegExt ${LANG_ENGLISH} "Associate ${PRODUCT_NAME} with the '${FILE_ASSO_EXT}' file extension."

  ;Assign language strings to sections
  !insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
    !insertmacro MUI_DESCRIPTION_TEXT ${SecRegExt} $(DESC_SecRegExt)
  !insertmacro MUI_FUNCTION_DESCRIPTION_END

;--------------------------------

