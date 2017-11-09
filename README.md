# MediaFilesManager

Simple cross platform plugin to access images and videos on Android and iOS.

**Platform Support**

|Platform|Version|
| ------------------- | :-----------: |
|Xamarin.iOS|iOS 8+|
|Xamarin.Android|API 14+|

### API Usage

Call **CrossMediaFileManager.Current.GetAlbumsAsync()**
```csharp
Task<IMediaAlbum[]> GetAlbumsAsync();
```

The result is an array of all device's imagens and videos albums (IMediaAlbum[])

```csharp
public interface IMediaAlbum
{
    string Title { get; }

    IMediaFile[] GetAllMediaFiles(MediaAlbumContentType contentTypes = MediaAlbumContentType.All);
    
    MediaAlbumContentType GetContentTypes();
    
    IMediaFile GetMediaFileAtIndex(int index, MediaAlbumContentType contentTypes = MediaAlbumContentType.All);
    
    int GetMediaFilesCount(MediaAlbumContentType contentTypes = MediaAlbumContentType.All);
}
```

To get all images and videos from an album:
```csharp
IMediaFile[] files = album.GetAllMediaFiles(MediaAlbumContentType.Images | MediaAlbumContentType.Videos);
```
```csharp
public interface IMediaFile
{
    DateTime CreationDate { get; }
    
    long Size { get; }
    
    MediaFileType Type { get; }

    Stream GetStream();
}

public interface IMediaFileWithImage
{
    ImageStream GetImage();
    
    ImageStream GetImage(int width, int height);
    
    ImageStream GetImage(MediaFileGetImageOptions options);
}

public interface IMediaFileImage : IMediaFile, IMediaFileWithImage
{
    DateTime TakenDate { get; }
    
    int Width { get; }
    
    int Height { get; }
    
    double Latitude { get; }
    
    double Longitude { get; }
}

public interface IMediaFileVideo : IMediaFile, IMediaFileWithImage
{
    DateTime TakenDate { get; }
    
    int Width { get; }
    
    int Height { get; }
    
    double Latitude { get; }
    
    double Longitude { get; }
    
    TimeSpan Duration { get; }
    
    string Artist { get; }
}
```

### Android specific in your BaseActivity or MainActivity (for Xamarin.Forms) add this code:
```csharp
protected override void OnCreate(Bundle bundle)
{
    base.OnCreate(bundle);

    global::Xamarin.Forms.Forms.Init(this, bundle);

    MediaFileManager.Initialize(Forms.Context);

    LoadApplication(new App());
}

public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
{
    base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

    MediaFileManager.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
}
```
