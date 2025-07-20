using System;

[Serializable]
public class Photo
{
    public int pixel_id;
    public int placenum;
}

[Serializable]
public class PhotoListWrapper
{
    public Photo[] photos;
}
