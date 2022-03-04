// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace WBid.WBidiPad.iOS
{
    [Register ("SortViewController")]
    partial class SortViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnAdvancePay { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnAdvancePayBlock { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnAdvancePayDutty { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnAdvancePayPerDay { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnAdvancePayTafb { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnAdvanePayFdp { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnBlockAmPm { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnBlockDaysOff { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnBlockPay { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnBlockPayDutty { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnBlockPerDiem { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnBlockPmAm { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnBlockVacPay { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnBlockWeekday { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnDoneBlockSort { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnPaySingle { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIScrollView scrollAdvance { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISegmentedControl sgSelectSort { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISwitch swAdvance { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISwitch swSingleAdvance { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView tvBlockSort { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView viewBlockSort { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView viewSinglePay { get; set; }

        [Action ("OnAdvancePayBlockClickEvent:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void OnAdvancePayBlockClickEvent (UIKit.UIButton sender);

        [Action ("OnAdvancePayClickEvent:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void OnAdvancePayClickEvent (UIKit.UIButton sender);

        [Action ("OnAdvancePayDuttyClickEvent:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void OnAdvancePayDuttyClickEvent (UIKit.UIButton sender);

        [Action ("OnAdvancePayFdpClickEvent:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void OnAdvancePayFdpClickEvent (UIKit.UIButton sender);

        [Action ("OnAdvancePayPerDayClickEvent:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void OnAdvancePayPerDayClickEvent (UIKit.UIButton sender);

        [Action ("OnAdvancePayTafbClickEvent:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void OnAdvancePayTafbClickEvent (UIKit.UIButton sender);

        [Action ("OnBlockAmPmClickEvent:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void OnBlockAmPmClickEvent (UIKit.UIButton sender);

        [Action ("OnBlockDaysOffClickEvent:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void OnBlockDaysOffClickEvent (UIKit.UIButton sender);

        [Action ("OnBlockPayClickEvent:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void OnBlockPayClickEvent (UIKit.UIButton sender);

        [Action ("OnBlockPayDuttyClickEvent:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void OnBlockPayDuttyClickEvent (UIKit.UIButton sender);

        [Action ("OnBlockPerDiemClickEvent:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void OnBlockPerDiemClickEvent (UIKit.UIButton sender);

        [Action ("OnBlockPmAmClickEvent:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void OnBlockPmAmClickEvent (UIKit.UIButton sender);

        [Action ("OnBlockVacPayClickEvent:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void OnBlockVacPayClickEvent (UIKit.UIButton sender);

        [Action ("OnBlockWeekdayClickEvent:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void OnBlockWeekdayClickEvent (UIKit.UIButton sender);

        [Action ("OnDoneBlockSortClickEvent:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void OnDoneBlockSortClickEvent (UIKit.UIButton sender);

        [Action ("OnPaySingleClickEvent:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void OnPaySingleClickEvent (UIKit.UIButton sender);

        [Action ("OnSegSelectSortValueChange:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void OnSegSelectSortValueChange (UIKit.UISegmentedControl sender);

        [Action ("OnSingleAdvanceChangeValue:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void OnSingleAdvanceChangeValue (UIKit.UISwitch sender);

        [Action ("OnSwitchAdvanceChangeValue:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void OnSwitchAdvanceChangeValue (UIKit.UISwitch sender);

        void ReleaseDesignerOutlets ()
        {
            if (btnAdvancePay != null) {
                btnAdvancePay.Dispose ();
                btnAdvancePay = null;
            }

            if (btnAdvancePayBlock != null) {
                btnAdvancePayBlock.Dispose ();
                btnAdvancePayBlock = null;
            }

            if (btnAdvancePayDutty != null) {
                btnAdvancePayDutty.Dispose ();
                btnAdvancePayDutty = null;
            }

            if (btnAdvancePayPerDay != null) {
                btnAdvancePayPerDay.Dispose ();
                btnAdvancePayPerDay = null;
            }

            if (btnAdvancePayTafb != null) {
                btnAdvancePayTafb.Dispose ();
                btnAdvancePayTafb = null;
            }

            if (btnAdvanePayFdp != null) {
                btnAdvanePayFdp.Dispose ();
                btnAdvanePayFdp = null;
            }

            if (btnBlockAmPm != null) {
                btnBlockAmPm.Dispose ();
                btnBlockAmPm = null;
            }

            if (btnBlockDaysOff != null) {
                btnBlockDaysOff.Dispose ();
                btnBlockDaysOff = null;
            }

            if (btnBlockPay != null) {
                btnBlockPay.Dispose ();
                btnBlockPay = null;
            }

            if (btnBlockPayDutty != null) {
                btnBlockPayDutty.Dispose ();
                btnBlockPayDutty = null;
            }

            if (btnBlockPerDiem != null) {
                btnBlockPerDiem.Dispose ();
                btnBlockPerDiem = null;
            }

            if (btnBlockPmAm != null) {
                btnBlockPmAm.Dispose ();
                btnBlockPmAm = null;
            }

            if (btnBlockVacPay != null) {
                btnBlockVacPay.Dispose ();
                btnBlockVacPay = null;
            }

            if (btnBlockWeekday != null) {
                btnBlockWeekday.Dispose ();
                btnBlockWeekday = null;
            }

            if (btnDoneBlockSort != null) {
                btnDoneBlockSort.Dispose ();
                btnDoneBlockSort = null;
            }

            if (btnPaySingle != null) {
                btnPaySingle.Dispose ();
                btnPaySingle = null;
            }

            if (scrollAdvance != null) {
                scrollAdvance.Dispose ();
                scrollAdvance = null;
            }

            if (sgSelectSort != null) {
                sgSelectSort.Dispose ();
                sgSelectSort = null;
            }

            if (swAdvance != null) {
                swAdvance.Dispose ();
                swAdvance = null;
            }

            if (swSingleAdvance != null) {
                swSingleAdvance.Dispose ();
                swSingleAdvance = null;
            }

            if (tvBlockSort != null) {
                tvBlockSort.Dispose ();
                tvBlockSort = null;
            }

            if (viewBlockSort != null) {
                viewBlockSort.Dispose ();
                viewBlockSort = null;
            }

            if (viewSinglePay != null) {
                viewSinglePay.Dispose ();
                viewSinglePay = null;
            }
        }
    }
}