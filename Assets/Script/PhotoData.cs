using System.Collections.Generic;

[System.Serializable]
public class Photo
{
    public int plant_id;
    public int user_id;
    public int placenum;       // 0=인벤토리, 1~N=슬롯
    public string image_url;   // plants.pixel_image_url (https)
    public string s3_key;      // 호환용(빈 문자열 올 수 있음)
}

[System.Serializable]
public class PhotoListWrapper
{
    public List<Photo> photos;
}

[System.Serializable]               // ✅ CHANGED: 저장용 페이로드 추가
public class SaveItem
{
    public int plant_id;
    public int placenum;
}

[System.Serializable]
public class SavePayload
{
    public List<SaveItem> photos;
}