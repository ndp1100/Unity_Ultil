//
//  SohaSDKPlugin.c
//  Unity-iPhone
//
//  Created by Duong TRAN Thai on 10/10/13.
//
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import <UIKit/UIImage.h>

//#import <SohaSDK/Soha.h>
//#import <SohaSDK/SohaUser.h>
//#import "SohaSDKPlugin/SohaSDKPlugin.h"
#import <MaoriStudio/MaoriStudioManager.h>
#import <MaoriStudio/ObjectUser.h>
#import <MaoriStudio/ObjectSetting.h>


#import "../UnityAppController.h"

void (*OnMobGameLoginSuccess)(const char *uIdToken) = NULL;
void (*OnMobGameLoginFail)() = NULL;
void (*OnMobGameLogoutSuccess)() = NULL;
void (*OnMobGamePaymentSuccess)(const char *data) = NULL;
void (*OnMobGamePaymentFail)(const char *message) = NULL;

// Convert C style string to NSString
NSString *CreateNSString(const char* string)
{
    if (string)
        return [NSString stringWithUTF8String:string];
    else
        return [NSString stringWithUTF8String:""];
    
}

char *MakeStringCopy(const char* string)
{
    if (string == NULL)
        return NULL;
    
    char *res = (char *) malloc(strlen(string) + 1);
    
    strcpy(res, string);
    
    return res;
}

UIImage *UIImageFromData(const uint8_t* data, int width, int height, float scale)
{
    int dataLength = width * height * 4;
    GLubyte *dataGL = (GLubyte *)malloc(dataLength * sizeof(GLubyte));
    
    for (int i = 0; i < dataLength; ++i) {
        dataGL[i] = data[i];
    }
    
    CGDataProviderRef ref = CGDataProviderCreateWithData(NULL, data, dataLength, NULL);
    CGColorSpaceRef colorspace = CGColorSpaceCreateDeviceRGB();
    CGImageRef iref = CGImageCreate(width, height, 8, 32, width * 4, colorspace, kCGBitmapByteOrder32Big | kCGImageAlphaPremultipliedLast, ref, NULL, true, kCGRenderingIntentDefault);
    
    // Create a graphics context with target size measured in POINTS
    NSInteger widthInPoints, heightInPoints;
    if (NULL != UIGraphicsBeginImageContextWithOptions)
    {
        widthInPoints = width / scale;
        heightInPoints = height / scale;
        
        
        UIGraphicsBeginImageContextWithOptions(CGSizeMake(widthInPoints, heightInPoints), NO, scale);
    }
    else
    {
        widthInPoints = width;
        widthInPoints = height;
        
        UIGraphicsBeginImageContext(CGSizeMake(widthInPoints, heightInPoints));
    }
    
    CGContextRef cgContext = UIGraphicsGetCurrentContext();
    
    CGContextSetBlendMode(cgContext, kCGBlendModeCopy);
    CGContextDrawImage(cgContext, CGRectMake(0, 0, widthInPoints, heightInPoints), iref);
    
    UIImage *image = UIGraphicsGetImageFromCurrentImageContext();
    
    UIGraphicsEndImageContext();
    
    free(dataGL);
    
    CFRelease(ref);
    CFRelease(colorspace);
    CGImageRelease(iref);
    
    return image;
}

//void _ShareFacebookImage(const char* message, const uint8_t* image, int width, int height, OnShareFacebookCompleteCallback onShareFacebookComplete)
//{
//    NSLog(@"[Soha shareFacebookMessage:image:url:block:^(SohaFacebookPostResult result, NSError *error)] is obsoleted");
//    float scale = GetAppController().rootView.contentScaleFactor;
//    UIImage *img = UIImageFromData(image, width, height, scale);
//
//    [Soha shareFacebookMessage:CreateNSString(message)
//                         image:img
//                           url:NULL
//                         block:^(SohaFacebookPostResult result, NSError *error) {
//                             NSMutableString *str = [[NSMutableString alloc] initWithString:@""];
//                             
//                             if (result != SohaFacebookPostResultDone)
//                             {
//                                 [str appendString: @"FAIL"];
//                                 if (error != NULL)
//                                 {
//                                     [str appendFormat:@" - %@", error.description];
//                                     NSLog(@"post facebook error : %@", error.description);
//                                 }
//                             }
//                             else
//                             {
//                                 [str appendString:@"SUCCESS"];
//                             }
//                             
//                             //UnitySendMessage("SohaSDKManager", "OnShareFacebookComplete", str.UTF8String);
//                             if (onShareFacebookComplete != NULL)
//                                 onShareFacebookComplete(MakeStringCopy(str.UTF8String));
//                         }];
//}

void _RequestLoginSoha(OnLoginSuccessCallback onLoginSuccess, OnLoginFailCallback onLoginFail)
{
    OnMobGameLoginFail = onLoginFail;
    OnMobGameLoginSuccess = onLoginSuccess;
    
//    [Soha sohaLoginWithBlock:^(SohaUser *user, NSError *error) {
//     
//     NSLog(@"User name: %@", user.userName);
//     
//     }];
    
    [MaoriStudioManager loginSDKWithBlock:^(ObjectUser *user, NSError *error) {
        // handler login
        NSLog(@"User name: %@", user.userName);
    }];
}

void _RequestLogoutSoha(OnLogoutSuccessCallback onLogoutSuccess)
{
    OnMobGameLogoutSuccess = onLogoutSuccess;
    NSLog(@"Request logout soha");
//    [Soha sohaLogoutSDK];
    [MaoriStudioManager CallLogoutSDK];
}

//#if SOHASDK_045
void _SetServerIdNameAndUserIdName(const char *areaId, const char *areaName, const char *roleId, const char *roleName, const char *level)
{
    NSLog(@"SetAreaId %@ - roleId %@", CreateNSString(areaId), CreateNSString(roleId));
    NSLog(@"SetAreaName %@ - roleName %@", CreateNSString(areaName), CreateNSString(roleName)); // thu hardcode cái này với 1 value gì đó xem: test chẳng hạn
//    [Soha sohaSetServerID:CreateNSString(areaId)
//               serverName:CreateNSString(areaName)
//              characterID:CreateNSString(roleId)
//            characterName:CreateNSString(roleName)
//           characterLevel:CreateNSString(level)];
    
    [MaoriStudioManager setGameServerID:CreateNSString(areaId)
                        gameServerName:CreateNSString(areaName)
                        characterID:CreateNSString(roleId)
                        characterName:CreateNSString(roleName)
                         characterLevel:CreateNSString(level)];
}

void _AddSohaDashBoardButtonLeftAnchor(float yfromTop)
{
    // add dashboard soha button
    
//    int height = [UIScreen mainScreen].bounds.size.height;
//    
//    if (UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPad)
//    {
//        [Soha addDashboardButtonInView: GetAppController().rootView
//                               atPoint: CGPointMake(37.5, yfromTop * height)
//                              withSize: CGSizeMake(75, 75)
//                               canMove:YES];
//        NSLog(@"Add mysohabutton at position (%f, %f)", 25.0f, yfromTop * height);
//    }
//    else
//    {
//        [Soha addDashboardButtonInView: GetAppController().rootView
//                               atPoint: CGPointMake(20, yfromTop * height)
//                              withSize: CGSizeMake(40, 40)
//                               canMove:YES];
//        NSLog(@"Add mysohabutton at position (%f, %f)", 15.0f, yfromTop * height);
//    }
}

void _HideSohaDashBoardButton()
{
    NSLog(@"Ẩn mysoha button");
//    [Soha sohaideDashboardButton:YES];
}

void _ShowSohaDashBoardButton()
{
    NSLog(@"Hiện mysoha button");
//    [Soha hideDashboardButton:NO];
}

//#else
//
//void _OnSohaUpdateSNSNotificationCount(const char *count)
//{
//    if (count != NULL)
//    {
//        UnitySendMessage("SohaSDKManager", "OnSohaUpdateSNSNotificationCount", count);
//    }
//    else
//    {
//        NSLog(@"_OnSohaUpdateSNSNotificationCount: count = null");
//    }
//}
//
//void _SetServerIdAndUserId(const char *areaId, const char *roleId)
//{
//    NSLog(@"[Soha setAreId:roleId:] đã bị loại bỏ dùng [Soha setAreaId:areaName:roleId:roleName:]");
//    [Soha setAreaId:CreateNSString(areaId) roleId:CreateNSString(roleId)];
//}
//void _SetServerNameAndUserName(const char *areaName, const char *roleName)
//{
//    NSLog(@"[Soha setAreaName:roleName:] đã bị loại bỏ dùng [Soha setAreaId:areaName:roleId:roleName:]");
//    [Soha setAreaName:CreateNSString(areaName) roleName:CreateNSString(roleName)];
//}
//
//#endif



//void _RequestPaymentSoha(OnPaymentSuccessCallback onPaymentSuccess, OnPaymentFailCallback onPaymentFail)
void _RequestPaymentSoha()
{
//    OnMobGamePaymentSuccess = onPaymentSuccess;
//    OnMobGamePaymentFail = onPaymentFail;
    
//    [Soha sohaInAppPhurchase:packIAP_None];
    [MaoriStudioManager showPayCenter];
    
//    NSString *bundleIdentifier = [[NSBundle mainBundle] bundleIdentifier];
//    
//    if ([bundleIdentifier compare:@"com.sohagame.dmc"] == 0)
//    {
//        NSLog(@"_Request Payment Soha");
//        [Soha showSohaPayPopup];
//    }
//    else
//    {
//        NSLog(@"_Request IAPPayment Soha");
//        [Soha showIAPShopPopup];
//    }
}

//void _WillDisconnectSoha()
//{
//    [Soha willDisconnect];
//}

//void _ShowDashBoardSoha()
//{
//    [Soha showDashBoard];
//}
//
//void _ShowForum()
//{
//    [Soha showForum];
//}
//
//void _ShowMySoha()
//{
//    [Soha showMyProfile];
//}
//
//void _ShowProfileSoha(const char *userNameSoha)
//{
//    [Soha showUserProfile: CreateNSString(userNameSoha)];
//}
//
//void _StartPaymentSoha()
//{
//    [Soha showLoadingPayment];
//}
//
//void _FinishPaymentSoha()
//{
//    [Soha hideLoadingPayment];
//}

//void _OnSohaDidShowDashBoard()
//{
//    UnitySendMessage("SohaSDKManager", "OnSohaDidShowDashBoard", "");
//}
//
//void _OnSohaDidDismissDashBoard()
//{
//    UnitySendMessage("SohaSDKManager", "OnSohaDidDismissDashBoard", "");
//}

void _OnSohaDidLoginUser(const char *userIdToken)
{
    if (OnMobGameLoginSuccess != NULL)
    {
        OnMobGameLoginSuccess(MakeStringCopy(userIdToken));
    }
//    UnitySendMessage("SohaSDKManager", "OnSohaLoginSuccess", idToken);
}

void _OnSohaLoginFail()
{
    if (OnMobGameLoginFail != NULL)
    {
        OnMobGameLoginFail();
    }
//    UnitySendMessage("SohaSDKManager", "OnSohaLoginFail", "");
}

void _OnSohaLogoutSuccess()
{
    if (OnMobGameLogoutSuccess != NULL)
    {
        OnMobGameLogoutSuccess();
    }
    //UnitySendMessage("SohaSDKManager", "OnSohaLogoutSuccess", "");
}

void _OnSohaPaymentSuccess(const char *data)
{
    if (OnMobGamePaymentSuccess != NULL)
    {
        OnMobGamePaymentSuccess(MakeStringCopy(data));
    }
    //UnitySendMessage("SohaSDKManager", "OnSohaPaymentSuccess", (data));
}

void _OnSohaPaymentFail()
{
    if (OnMobGamePaymentSuccess != NULL)
    {
        OnMobGamePaymentSuccess(MakeStringCopy("payment failed"));
    }
    //UnitySendMessage("SohaSDKManager", "OnSohaPaymentFail", "");
}

int _IsJailBreak()
{
    NSString *bundleIdentifier = [[NSBundle mainBundle] bundleIdentifier];
    if ([bundleIdentifier compare:@"com.sohagame.sg34"] == 0)
    {
        return 1;
    }
    else
        return 0;
}

const char* _GetBundleIdentifier()
{
    NSString *bundleIdentifier = [[NSBundle mainBundle] bundleIdentifier];

    return MakeStringCopy(bundleIdentifier.UTF8String);
}

const char* _GetAppIdFromBundleInfo()
{
    NSString *sohaAppId = [[NSBundle mainBundle] objectForInfoDictionaryKey: @"SohaAppID"];
    
    return MakeStringCopy(sohaAppId.UTF8String);
}
