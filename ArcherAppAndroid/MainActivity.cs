using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Content;
using Android.Speech;
using System.Collections.Generic;
using AlertDialog = Android.App.AlertDialog;
using Android;
using System.Net.Http;
using System;
using System.Net;
using System.Threading.Tasks;
using Android.Views;



namespace ArcherAppAndroid
{
    [Activity(Label = "Archer", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        //"@string/app_name"
        private TextView mFilterText;
        private TextView textIndicator;
        private Button mRecordButton;
        private ProgressBar progressBar;
        private bool isRecording;

        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            // get the resources from the layout
            mRecordButton = FindViewById<Button>(Resource.Id.BtnRecord);
            mFilterText = FindViewById<TextView>(Resource.Id.txtRecord);
            textIndicator = FindViewById<TextView>(Resource.Id.txtIndicator);
            progressBar = FindViewById<ProgressBar>(Resource.Id.Progress);


            mRecordButton.Text = "Tap to Speak Archer";
            mFilterText.Text = "Speak to Archer....";
            progressBar.Visibility = ViewStates.Invisible;
            textIndicator.Visibility = ViewStates.Invisible;
            

            string rec = Android.Content.PM.PackageManager.FeatureMicrophone;
            //checking for Microphone on device  
            if (rec != "android.hardware.microphone")
            {
                var micAlert = new AlertDialog.Builder(mRecordButton.Context);
                micAlert.SetTitle("Device doesn't have a mic for recording");
                micAlert.SetPositiveButton("OK", (sender, e) =>
                {
                    return;
                });
                micAlert.Show();
                
            }

            else
                mRecordButton.Click += delegate
                {
                    isRecording = !isRecording;
                    if (isRecording)
                    {
                        // create the voice intent  
                        var voiceIntent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
                        voiceIntent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);

                        // message and modal dialog  
                        voiceIntent.PutExtra(RecognizerIntent.ExtraPrompt, "Speak now");

                        // end capturing speech if there is 3 seconds of silence  
                        voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputCompleteSilenceLengthMillis, 500);
                        voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputPossiblyCompleteSilenceLengthMillis, 500);
                        voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputMinimumLengthMillis, 10000);
                        voiceIntent.PutExtra(RecognizerIntent.ExtraMaxResults, 1);

                        // method to specify other languages to be recognised here if desired  
                        voiceIntent.PutExtra(RecognizerIntent.ExtraLanguage, Java.Util.Locale.Default);
                        StartActivityForResult(voiceIntent, 10);
                        isRecording = false;
                    }
                };

        }

        protected override async void OnActivityResult(int requestCode, Result result, Intent data)
        {
            if (requestCode == 10)
            {
                if (result == Result.Ok)
                {
                    var matches = data.GetStringArrayListExtra(RecognizerIntent.ExtraResults);
                    if (matches.Count != 0)
                    {
                        //string textInput = mFilterText.Text + matches[0];
                        string textInput = matches[0];


                        //SendGetRequest("https://postman-echo.com/get?foo1=bar1&foo2=bar2");

                        
                        progressBar.Visibility = ViewStates.Visible;
                        textIndicator.Visibility = ViewStates.Visible;

                        //string s = await SendGetRequest("https://postman-echo.com/get?foo1=bar1&foo2=bar2");

                        //mFilterText.Text = s;

                        mFilterText.Text = textInput;
                        progressBar.Visibility = ViewStates.Invisible;
                        textIndicator.Visibility = ViewStates.Invisible;
                        


                    }
                    else
                        mFilterText.Text = "Nothing was recognized";
                }
            }

            base.OnActivityResult(requestCode, result, data);
        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        
        private async Task<string> SendGetRequest(string message)
        {
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri(message);
            request.Method = HttpMethod.Get;

            var client = new HttpClient();
            HttpResponseMessage response = await client.SendAsync(request);

            HttpContent content = response.Content;
            string json = await content.ReadAsStringAsync();

            return await Task.FromResult(json);
    
            
        }
    }
}