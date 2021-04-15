using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using HoneywellRfidNative.Adapters;
using HoneywellRfidNative.Dto;
using HoneywellRfIdShared.Hub;
using HoneywellRfIdShared.Messaging;
using HoneywellRfIdShared.Messaging.Messages;
using Messenger;
using Xamarin.Essentials;

namespace HoneywellRfidNative
{
    [Activity(Label = "RfIdReaderActivity")]
    public class RfIdReaderActivity : Activity
    {
        private ListView _rfidListview;
        private Button _rfidScanButton;

        private RfidTagListAdapter _tagListAdapter;

        private MessengerSubscriptionToken _rfIdTriggeredEventToken;
        private MessengerSubscriptionToken _tagEventToken;

        private Dictionary<string, TagDisplayItem> _scannedTags = new Dictionary<string,TagDisplayItem>();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.Window.SetFlags(WindowManagerFlags.KeepScreenOn, WindowManagerFlags.KeepScreenOn);

            SetContentView(Resource.Layout.activity_rfidreader);

            _rfidListview = FindViewById<ListView>(Resource.Id.rfidListView);
            _rfidScanButton = FindViewById<Button>(Resource.Id.rfidScanButton);

            _rfidScanButton.Click += RfidScanButton_Click;

            _tagListAdapter = new RfidTagListAdapter(this, ref _scannedTags);
            _rfidListview.Adapter = _tagListAdapter;
        }

        private void OnTagMessage(TagMessage message)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                

                foreach (var tag in message.TagReadDataReading)
                {
                    TagDisplayItem displayTag = null;

                    if (!_scannedTags.ContainsKey(tag.EpcHexStr))
                    {
                        displayTag = new TagDisplayItem();
                        _scannedTags[tag.EpcHexStr] = displayTag;
                        displayTag.Epc = tag.EpcHexStr;
                        displayTag.Count += tag.ReadCount;
                    }
                    else
                    {
                        displayTag = _scannedTags[tag.EpcHexStr];
                        displayTag.Epc = tag.EpcHexStr;
                        displayTag.Count += tag.ReadCount;
                    }
                }

                _tagListAdapter.NotifyDataSetChanged();
            });
        }

        private void OnRfidTriggerMessage(RfidTriggeredMessage message)
        {
            if (message.Triggered)
            {
                 _scannedTags.Clear();
                _tagListAdapter.NotifyDataSetChanged();


                HoneywellDeviceHub.GetInstance().StartReading();
            }
            else
            {
                HoneywellDeviceHub.GetInstance().StopReading();
            }

            UpdateUi();
        }

        private void RfidScanButton_Click(object sender, EventArgs e)
        {
            if (!HoneywellDeviceHub.GetInstance().IsReading)
                HoneywellDeviceHub.GetInstance().StartReading();
            else
                HoneywellDeviceHub.GetInstance().StopReading();

            UpdateUi();

        }

        private void UpdateUi()
        {
            if (HoneywellDeviceHub.GetInstance().IsReading)
                _rfidScanButton.Text = "Stop";
            else
                _rfidScanButton.Text = "Start";
        }

        //LIFE CYCLE CONTROL

        protected override void OnResume()
        {
            base.OnResume();

            HoneywellDeviceHub.GetInstance().Attach();

            _rfIdTriggeredEventToken = PubSubHandler.GetInstance().Subscribe<RfidTriggeredMessage>(OnRfidTriggerMessage);
            _tagEventToken = PubSubHandler.GetInstance().Subscribe<TagMessage>(OnTagMessage);
        }

        //LIFE CYCLE CONTROL
        protected override void OnPause()
        {
            PubSubHandler.GetInstance().Unsubscribe<TagMessage>(_tagEventToken);
            PubSubHandler.GetInstance().Unsubscribe<RfidTriggeredMessage>(_rfIdTriggeredEventToken);
            HoneywellDeviceHub.GetInstance().Detach();

            base.OnPause();
        }
    }
}