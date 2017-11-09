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

Get all images and videos from an album:
```csharp
IMediaFile[] files = album.GetAllMediaFiles(MediaAlbumContentType.Images | MediaAlbumContentType.Videos);
```

Get the file's thumbnail:
```csharp
 var file = files[0] as IMediaFileWithImage;

 ImageStream stream = file.GetImage(MediaFileGetImageOptions.CreateDefaultThumb());
```

Get custom image:
```csharp
var file = files[0] as IMediaFileWithImage;

var options = new MediaFileGetImageOptions
{
    Width = 500,
    Height = 500,
    Quality = 90,
    Orientation = MediaFileImageOrientation.Right,                          // Rotate image to right
    ResizeAspect = MediaFileGetImageOptions.ImageResizeAspect.AspectFit     
};

ImageStream stream = file.GetImage(options);
```

Get the full size file's stream:
```csharp
Stream stream = files[0].GetStream();
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
### Samples
![](https://raw.githubusercontent.com/FelipeNicoletto/MediaFilesManager/master/Images/image_1.png)
![](https://raw.githubusercontent.com/FelipeNicoletto/MediaFilesManager/master/Images/image_2.png)
![](https://raw.githubusercontent.com/FelipeNicoletto/MediaFilesManager/master/Images/image_3.png)
![](https://raw.githubusercontent.com/FelipeNicoletto/MediaFilesManager/master/Images/image_4.png)


