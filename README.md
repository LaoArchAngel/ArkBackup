# Introduction
This is a simple little app that allows you to watch your Ark saves and backs
keeps a rolling backup.  You can use these backups in either a server or
your local machine.  They can be used to debug server crashes, or just to
recover lost dinos / bases.

##  Requirements
This application REQUIRES a 64-bit computer.  This is due to our dependency in
7z (read more below).  We are currently only supporting 64-bit, but 32-bit will
come at a later date.  Not really sure why anyone would run Ark server on a
32-bit computer, but to each their own.

## Important Dependencies
This application comes packed with *7z.dll*.  This is used to compress the
backups.  You will need a full version of 7zip in order to decompress them.  
You can find the source and binaries for 7z.dll here:
[7-zip.org](http://www.7-zip.org/a/7z1604-src.7z)

## Usage
Just run `ArkBackup.exe`.  Hit enter when you want to close the application.

## Configuration
The application comes with a default configuration for TheIsland at the basic
desktop install location.  You can modify it or use this section to create
your own settings

### Required Fields
* __path__  - Path to the folder with Ark saves.

### Optional Fields
* __name__
  * __Default__: TheIsland
  * __Notes__: The save file you want to watch.  __DO NOT__ include the _.ark_
  extension.
* __saves__
  * __Default__: 10
  * __Notes__: The number of backups to keep before the last is deleted.
* __delay__
  * __Default__: 10
  * __Notes__: How long (in seconds) the application should wait before
  attempting to create the backup.  If you're having problems backing up files,
  try increasing this first.  Larger saves may warrant longer delays.

### Format
Note that `ArkBackupGroup` is required!  The application will crash without
it.  If there are no `ArkBackupConfig` lines, the application should run fine
but will do nothing.
```xml
  <ArkBackupGroup>
    <ArkBackupConfig name="TheIsland" path="C:\Path\To\Saves" saves="20" delay="60"/>
  </ArkBackupGroup>
```
### Examples
Minimal.  Assumes _TheIsland_ map / saves.  Keeps 10 backups and waits only 10
seconds.
```xml

  <ArkBackupGroup>
    <ArkBackupConfig path="C:\Program Files (x86)\Steam\steamapps\common\ARK\ShooterGame\Saved\SavedArksLocal"/>
  </ArkBackupGroup>
```

Specifying map.  Keeps 10 backups and waits only 10 seconds.
```xml

  <ArkBackupGroup>
    <ArkBackupConfig name="Ragnarok" path="C:\Program Files (x86)\Steam\steamapps\common\ARK\ShooterGame\Saved\SavedArksLocal"/>
  </ArkBackupGroup>
```

Two watchers.  One is TheIsland and keeps 20 backups.  The other is Ragnarok,
keeps 10 backups and waits a full minute before attempting the backup.
```xml
  <ArkBackupGroup>
    <ArkBackupConfig path="C:\arkserver\ShooterGame\Saved\SavedArks" saves="20"/>
    <ArkBackupConfig name="Ragnarok" path="C:\arkserver2\ShooterGame\Saved\SavedArks" delay="60"/>
  </ArkBackupGroup>
```
