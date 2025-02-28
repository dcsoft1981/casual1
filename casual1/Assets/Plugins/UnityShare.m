#import <UIKit/UIKit.h>

#ifdef __cplusplus
extern "C" {
#endif

void _ShowShareSheet(const char* message)
{
    NSString *text = [NSString stringWithUTF8String:message];
    dispatch_async(dispatch_get_main_queue(), ^{
        UIViewController *rootViewController = [UIApplication sharedApplication].keyWindow.rootViewController;
        if (!rootViewController) {
            NSLog(@"루트 뷰 컨트롤러를 찾을 수 없습니다.");
            return;
        }

        NSArray *items = @[text];
        UIActivityViewController *activityVC = [[UIActivityViewController alloc] initWithActivityItems:items applicationActivities:nil];

        if (UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPad) {
            activityVC.popoverPresentationController.sourceView = rootViewController.view;
            activityVC.popoverPresentationController.sourceRect = CGRectMake(rootViewController.view.bounds.size.width/2, rootViewController.view.bounds.size.height, 0, 0);
        }

        [rootViewController presentViewController:activityVC animated:YES completion:nil];
    });
}

#ifdef __cplusplus
}
#endif

