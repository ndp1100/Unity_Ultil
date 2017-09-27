//
//  SohaSDKPlugin.h
//  Unity-iPhone
//
//  Created by Duong TRAN Thai on 10/10/13.
//
//

#ifndef Unity_iPhone_SohaSDKPlugin_h
#define Unity_iPhone_SohaSDKPlugin_h

//#define SOHASDK_045 1

extern "C" {
    typedef void (*OnLoginSuccessCallback)(const char *);
    typedef void (*OnLoginFailCallback)();
    typedef void (*OnLogoutSuccessCallback)();
    typedef void (*OnPaymentSuccessCallback)(const char *);
    typedef void (*OnPaymentFailCallback)(const char *);
//    typedef void (*OnShareFacebookCompleteCallback)(const char *);
    
    extern OnLoginSuccessCallback OnMobGameLoginSuccess;
    extern OnLoginFailCallback OnMobGameLoginFail;
    extern OnLogoutSuccessCallback OnMobGameLogoutSuccess;
//    extern OnPaymentSuccessCallback OnMobGamePaymentSuccess;
//    extern OnPaymentFailCallback OnMobGamePaymentFail;
    
    void _RequestLoginSoha(OnLoginSuccessCallback onLoginSuccess, OnLoginFailCallback onLoginFail);
    
    void _RequestLogoutSoha(OnLogoutSuccessCallback onLogoutSuccess);
    
    void _RequestPaymentSoha();
    
//    void _FinishPaymentSoha();
//    
//    void _WillDisconnectSoha();
    
//    void _ShowDashBoardSoha();
//    
//    void _ShowForum();
//    
//    void _ShowMySoha();
//    
//    void _ShowProfileSoha(const char *userNameSoha);
    
//#if SOHASDK_045
    void _SetServerIdNameAndUserIdName(const char *areaId, const char *areaName,
                                       const char *roleId, const char *roleName,
                                       const char * level);
    
    void _AddSohaDashBoardButtonLeftAnchor(float yfromTop);
    void _HideSohaDashBoardButton();
    void _ShowSohaDashBoardButton();
//#else
//    
//    void _SetServerIdAndUserId(const char *areaId, const char *roleId);
//    void _SetServerNameAndUserName(const char *areaName, const char *roleName);
//    
//    void _OnSohaUpdateSNSNotificationCount(const char *count);
//#endif
    
//    void _OnSohaDidShowDashBoard();
//    
//    void _OnSohaDidDismissDashBoard();
    
    void _OnSohaDidLoginUser(const char *userIdToken);
    
    void _OnSohaLoginFail();
    
    void _OnSohaLogoutSuccess();
    
//    void _OnSohaPaymentSuccess(const char *data);
    
//    void _OnSohaPaymentFail();
    
    int _IsJailBreak();
    
    const char* _GetBundleIdentifier();
    
    const char* _GetAppIdFromBundleInfo();
    
//    void _ShareFacebookImage(const char* message, const uint8_t* image, int width, int height, OnShareFacebookCompleteCallback onShareFacebookComplete);
}
#endif
