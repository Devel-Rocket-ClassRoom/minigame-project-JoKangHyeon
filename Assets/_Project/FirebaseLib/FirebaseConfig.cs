using UnityEngine;

[CreateAssetMenu(fileName = "FirebaseConfig", menuName = "Firebase Study/Firebase Config")]
public class FirebaseConfig : ScriptableObject
{
    [Tooltip("웹 API 키 (프로젝트 설정 > 일반)")]
    public string apiKey;

    [Tooltip("앱 ID (mobilesdk_app_id, 예: 1:1234567890:android:abcdef)")]
    public string appId;

    [Tooltip("프로젝트 ID")]
    public string projectId;

    [Tooltip("Realtime Database URL (예: https://your-project-default-rtdb.firebaseio.com)")]
    public string databaseUrl;

    [Tooltip("스토리지 버킷 (선택)")]
    public string storageBucket;

    public bool IsValid => !string.IsNullOrEmpty(apiKey) && !string.IsNullOrEmpty(databaseUrl);
}