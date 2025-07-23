using System.Collections.Generic;

[System.Serializable]
public class Photo
{
    public int pixel_id;
    public string user_id;
    public int placenum;
    public string s3_key;
    public string image_url;
}

[System.Serializable]
public class PhotoListWrapper
{
    public List<Photo> photos;
}