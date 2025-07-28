using System.Collections.Generic;

[System.Serializable]
public class Photo
{
    public int plant_id;
    public int user_id;
    public int placenum;
    public string s3_key;
    public string image_url;
}

[System.Serializable]
public class PhotoListWrapper
{
    public List<Photo> photos;
}