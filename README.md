# File System Watcher

A cross-platform background service with a file system watcher

## Build artifacts

```
dotnet publish -c Release -r linux-x64 --self-contained=true -p:PublishSingleFile=true -o artifacts
```

## Building binary deb package

### Package structure

```
filewatcher_<version>_amd64
  | 
  ├─DEBIAN
  |   control
  |   config
  |   templates
  |   postinst
  |   prerm
  ├─usr
  |  ├─sbin
  |     ├─filewatcher
  |     |   FileSystemWatcher.Host
  |     |   appsettings.json
  ├─etc
  |  ├─systemd
  |     ├─system
  |     |   filewatcher.service
```

### Permissions

```
sudo chmod 0755 filewatcher_<version>_amd64/DEBIAN/config
sudo chmod 0755 filewatcher_<version>_amd64/DEBIAN/postinst
sudo chmod 0755 filewatcher_<version>_amd64/DEBIAN/prerm
```

### Build the package

```
dpkg-deb --build filewatcher_<version>_amd64/
```

### Test the package

```
sudo dpkg -i filewatcher_<version>_amd64.deb
sudo systemctl status filewatcher.service
sudo dpkg -r filewatcher
```

### Regenerate debconf

```
sudo rm -r /var/cache/debconf
sudo mkdir /var/cache/debconf
```
