using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using AOT;
using System;

public class SohaSDKManager : MonoBehaviour
{
#if SOHA_SDK
    public static SohaSDKManager instance;
    public static bool isSohaBtnActive= false;
    public string countNotification = string.Empty;
    public System.Action<string> onSohaUpdateNotification;
#if UNITY_ANDROID
	AndroidJavaClass jc;
	
#elif UNITY_IOS
	[DllImport("__Internal")]
	private static extern int _IsJailBreak();
	[DllImport("__Internal")]
	private static extern void _RequestLoginSoha(OnLoginSuccessCallback onLoginSuccess, OnLoginFailCallback onLoginFail);
	[DllImport("__Internal")]
	private static extern void _RequestLogoutSoha(OnLogoutSuccessCallback onLogoutSuccess);
//	[DllImport("__Internal")]
//	private static extern void _SetServerIdAndUserId(string areaId, string roleId);
//	[DllImport("__Internal")]
//	private static extern void _SetServerNameAndUserName(string areaName, string roleName);
	[DllImport("__Internal")]
	private static extern void _SetServerIdNameAndUserIdName(string areaId, string areaName, string roleId, string roleName, string level);
	[DllImport("__Internal")]
	private static extern void _AddSohaDashBoardButtonLeftAnchor(float yfromTop);
	[DllImport("__Internal")]
	private static extern void _HideSohaDashBoardButton();
	[DllImport("__Internal")]
	private static extern void _ShowSohaDashBoardButton();

	[DllImport("__Internal")]
	private static extern void _RequestPaymentSoha();
	//private static extern void _RequestPaymentSoha(OnPaymentSuccessCallback onPaymentSuccess, OnPaymentFailCallback onPaymentFail);

//	[DllImport("__Internal")]
//	private static extern void _FinishPaymentSoha();
//	[DllImport("__Internal")]
//	private static extern void _WillDisconnectSoha();
    //[DllImport("__Internal")]
    //private static extern void _ShowDashBoardSoha();
    //[DllImport("__Internal")]
    //private static extern void _ShowForum();
    //[DllImport("__Internal")]
    //private static extern void _ShowMySoha();
    //[DllImport("__Internal")]
    //private static extern void _ShowProfileSoha(string userNameSoha);
//	[DllImport("__Internal")]
//    private static extern void _ShareFacebookImage(string message, byte[] image, int width, int height);
#endif

#if UNITY_WINRT
    //call native
    public event Action LoginAction;
    public event Action<string, string, string, string> ConfigAction;
    public event Action LogoutAction;
    public event Action PaymentAction;
    public static bool IsLogout = false;
#endif
    // Use this for initialization
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Init();
        }

        DontDestroyOnLoad(instance.gameObject);
    }

#if UNITY_IOS //&& !UNITY_EDITOR
	public bool IsJailBreak()
	{
		return (_IsJailBreak() != 0);
	}
#endif

    public void Init()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
		jc = new AndroidJavaClass("vn.shg.mobi.mongvolam.UnityActivity");
		EGDebug.Log ("-----------------> " + jc);
#endif
    }

	/// <summary>
	/// Adds the soha button left anchor.
	/// </summary>
	/// <param name="yfromTop"> pos y value in (0 - 1).</param>
	public void AddSohaButtonLeftAnchor(float yfromTop)
	{
#if UNITY_IOS
		_AddSohaDashBoardButtonLeftAnchor( yfromTop);
#endif
	}

	public void ShowSohaDashBoardButton()
	{
#if UNITY_IOS
		_ShowSohaDashBoardButton();
        
#elif UNITY_ANDROID
        isSohaBtnActive = true;
        jc.CallStatic("RequestShowSoha");
#endif
    }

	public void HideSohaDashBoardButton()
	{
#if UNITY_IOS
		_HideSohaDashBoardButton();
#elif UNITY_ANDROID
        if (isSohaBtnActive)
        {
            jc.CallStatic("RequestHideSoha");
            isSohaBtnActive = false;
        }
#endif
    }

    public void SetUserInfo(string areaId, string roleId)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
		jc.CallStatic("SetUserConfig", areaId, roleId);
#elif UNITY_IOS && !UNITY_EDITOR
		//_SetServerIdAndUserId(areaId, roleId);
#endif
    }

    IEnumerator SetSohaUserInfo(int serverID, int gid, string token)
    {

        string login_url = string.Format("http://soap.soha.vn/api/a/GET/mobile/logplayuser?app_id=eefffb5332badbe91c91d395f6a03a13&areaid={0}&roleid={1}&gver=2.0.0&sdkver=0.0.0&clientname=sohagame&access_token={2}", serverID, gid, token);

        WWW www = new WWW(login_url);

        yield return www;
    }

	public void SetUserInfo(string areaId, string areaName, string roleId, string roleName, string level)
	{
#if UNITY_ANDROID && !UNITY_EDITOR        
		jc.CallStatic("SetUserConfig", areaId, roleId, roleName, level);
#elif UNITY_IOS && !UNITY_EDITOR
		_SetServerIdNameAndUserIdName(areaId, areaName, roleId, roleName, level);        
#elif WP_SDK
        ConfigAction(areaId, roleId, roleName, level);
#elif SOHA_SDK
        string token = PlayerPrefs.GetString("shtoken","");
        StartCoroutine(SetSohaUserInfo(GameManager.instance.ServerID, GameManager.instance.GamerID, token));
#endif
	}
    public void Login()
    {
#if UNITY_ANDROID
        jc.CallStatic("RequestLoginSoha");
        Debug.Log("Call login!");
#elif UNITY_IOS
		_RequestLoginSoha(OnSohaLoginSuccess, OnSohaLoginFail);
#elif UNITY_WINRT
        //if (PlayerPrefs.HasKey("logout") && PlayerPrefs.GetInt("logout") == 1)
        //{
        //    LogoutAction();

        //}
        //else
            LoginAction();
#endif
    }

    public void Logout()
    {
#if UNITY_ANDROID
        jc.CallStatic("RequestLogoutSoha");
#elif UNITY_IOS
		_RequestLogoutSoha(OnSohaLogoutSuccess);
#elif UNITY_WINRT && WP_SDK
        //IsLogout = true;
        //PlayerPrefs.SetInt("logout", 1);
        LogoutAction();

#endif
    }

    public void Payment()
    {
#if UNITY_ANDROID
        if (jc != null)
            jc.CallStatic("RequestPaymentSoha");
        else
        {

        }
#elif UNITY_IOS
		//_RequestPaymentSoha(OnSohaPaymentSuccess, OnSohaPaymentFail);
		_RequestPaymentSoha();
#elif UNITY_WINRT
        PaymentAction();
#endif
    }
    public bool IsLoggedFacebook()
    {
        return PlayerPrefs.GetInt(GameManager.instance.m_userName + "facebook", 0) != 0;
    }
    public void ShareFacebook()
    {
        PlayerPrefs.SetInt(GameManager.instance.m_userName + "facebook", 1);
#if UNITY_ANDROID
		//jc.CallStatic("ShareFacebook");
#endif
    }
    public void ShareFacebookUrl(string text, string title, string url, string content, string picture)
    {
        PlayerPrefs.SetInt(GameManager.instance.m_userName + "facebook", 1);
#if UNITY_ANDROID	
		//jc.CallStatic("ShareFacebookUrl", text, title, url, content, picture);
#endif
    }

    public void FinishPayment()
    {
//#if UNITY_IOS && !UNITY_EDITOR
//		_FinishPaymentSoha();
//#endif
    }

//    public void ShowDashBoard()
//    {
//#if UNITY_ANDROID	
//        //jc.CallStatic("RequestOpenDashboard");
//#endif
//#if UNITY_IOS
//        _ShowDashBoardSoha();
//#endif
//    }

//    void OnSohaUpdateSNSNotificationCount(string count)
//    {
//        if (count != null)
//        {
//            countNotification = count;
//            if (onSohaUpdateNotification != null)
//            {
//                onSohaUpdateNotification(countNotification);
//            }
//        }
//    }
//
//    void OnSohaDidShowDashBoard()
//    {
//        EGDebug.Log("OnSohaDidShowDashBoard");
//    }
//
//    void OnSohaDidDismissDashBoard()
//    {
//        EGDebug.Log("OnSohaDidDismissDashBoard");
//    }
    
#if UNITY_IOS
	delegate void OnLoginSuccessCallback(string idToken);
	[MonoPInvokeCallback(typeof(OnLoginSuccessCallback))]
	public static void OnSohaLoginSuccess(string idToken)
#else
    public void OnSohaLoginSuccess(string idToken)
#endif
    {
        PlayerPrefs.SetInt("login", 1);
        Debug.Log("Login success!");
        string[] strs = idToken.Split(" ".ToCharArray());
        string id = strs[0];
        string token = strs[1];

		//GameManager.instance.m_EntryClient.LoginSoha(id, token);

        GameManager.instance.m_userName = id;
        GameManager.instance.m_accessToken = token;

        GameManager.instance.m_EntryClient.IssueConnect();
        //HTTP.Instance.LoginSoha(new HTTP.OnRequest(HTTP.Instance.WaitForLogin), id, token);
    }
    
#if UNITY_IOS
	delegate void OnLogoutSuccessCallback();
	[MonoPInvokeCallback(typeof(OnLogoutSuccessCallback))]
	public static void OnSohaLogoutSuccess()
#else
    void OnSohaLogoutSuccess(string message)
#endif
    {
        //logout sucesss here
        PlayerPrefs.SetInt("login", 0);
        Debug.Log("Logout success!");
        GameManager.instance.m_GameClient.LogOffGameServer();
    }
    
#if UNITY_IOS
	delegate void OnLoginFailCallback();
	[MonoPInvokeCallback(typeof(OnLoginFailCallback))]
	public static void OnSohaLoginFail()
#else
    void OnSohaLoginFail(string message)
#endif
    {
        MessagePopup.Create("Xảy ra lỗi khi đăng nhập, xin vui lòng thử lại");
    }

    string lastOrderId = string.Empty;
    
#if UNITY_IOS
	delegate void OnPaymentSuccessCallback(string data);
	[MonoPInvokeCallback(typeof(OnPaymentSuccessCallback))]
	public static void OnSohaPaymentSuccess(string data)
#else
    public void OnSohaPaymentSuccess(string data)
#endif
    {
        string[] strs = data.Split(" ".ToCharArray());
        Debug.Log("Payment success " + data);
        if (strs.Length > 1)
        {
            string orderId = strs[1];

			string lastOrderId = SohaSDKManager.instance.lastOrderId;

            if (lastOrderId == orderId)
            {
                EGDebug.LogWarning("Dupplicate payment success callback. OrderId = " + orderId);
            }
            else
            {
                lastOrderId = orderId;

				PaymentRequest request = new PaymentRequest();
				request.OrderID = orderId;
				GameManager.instance.m_GameClient.RequestPaymentConfirm(request);
            }
        }
        //FinishPayment();
    }
    
#if UNITY_IOS
	delegate void OnPaymentFailCallback(string message);
	[MonoPInvokeCallback(typeof(OnPaymentFailCallback))]
	public static void OnSohaPaymentFail(string message)
#else
    void OnSohaPaymentFail(string message)
#endif
    {
		MessagePopup.Create(message);
    }

//    void OnShareFacebookImage(GameObject button)
//    {
//        StartCoroutine(CaptureScene(button, "Game kiếm hiệp cực hay\nDownload:http://minhchu.sohagame.vn/download", "", "", ""));
//    }
//
//    void OnShareFacebookComplete(string result)
//    {
//        if (result.StartsWith("SUCCESS"))
//        {
//            //MessagePopup.Create("Chia sẻ thành công");
//
//            //if (HTTP.Instance.UserInfo.Data.MoiRuou.ShareFB == 0)
//            //{
//            //    HTTP.Instance.NhanThuongShareFB(new HTTP.OnRequest(HTTP.Instance.WaitForNhanThuongShareFB));
//            //}
//        }
//        else
//        {
//            //MessagePopup.Create("Chia sẻ thất bại");
//        }
//    }

//    IEnumerator CaptureScene(GameObject button, string text, string title, string url, string content)
//    {
//        button.GetComponent<BoxCollider>().enabled = false;
//        yield return new WaitForEndOfFrame();
//
//        while (Application.isLoadingLevel)
//            if (Application.isLoadingLevel)
//            {
//                yield return new WaitForSeconds(1);
//            }
//
//
//
//        // create a texture to pass to encoding
//        Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
//
//        // put buffer into texture
//        texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
//        texture.Apply();
//
//#if UNITY_ANDROID	
//		/*JPGEncoder encoder = new JPGEncoder( texture, 50);
//		byte[] bytes = encoder.GetBytes();
//		
//		jc.CallStatic("ShareFacebookImage", text, title, url, content, bytes);*/
//#elif UNITY_IOS
//		
//		Color32[] colors = texture.GetPixels32();
//		
//		int dataLength = texture.width * texture.height * 4;
//		
//		byte[] imageData = new byte[dataLength];
//		
//		for (int j = 0; j < texture.height; ++j)
//		{
//			for (int i = 0; i < texture.width; ++i)
//			{
//				//int h = texture.height - j - 1;
//				
//				int p = j * texture.width * 4 + i * 4;
//				
//				Color32 color = colors[j * texture.width + i];
//				
//				imageData[p] = color.r;
//				imageData[p + 1] = color.g;
//				imageData[p + 2] = color.b;
//				imageData[p + 3] = color.a;
//			}
//		}
//		
//		_ShareFacebookImage(text, imageData, texture.width, texture.height);
//#endif
//
//        button.GetComponent<BoxCollider>().enabled = true;
//        yield return 0;
//    }
    public void RequestShowSoha()
    {
#if UNITY_ANDROID
        jc.CallStatic("RequestShowSoha");
#endif
    }
    public void RequestHideSoha()
    {
#if UNITY_ANDROID
        jc.CallStatic("RequestHideSoha");
#endif
    }

#endif
}

