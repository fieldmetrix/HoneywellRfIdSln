using System;
using Android.Content;
using Android.OS;
using Android.Views;
using HoneywellRfId.Interfaces;
using Java.Lang;

namespace HoneywellRfId.Fragments
{
    public class BaseFragment : AndroidX.Fragment.App.Fragment
    {
        private const string ARG_PARAM1 = "param1";
        private const string ARG_PARAM2 = "param2";

        private string _param1;
        private string _param2;

        private string _clsName;

        private IOnFragmentInteractionListener _listener;

        public static BaseFragment NewInstance(string param1, string param2)
        {
            BaseFragment fragment = new BaseFragment();
            Bundle args = new Bundle();
            args.PutString(ARG_PARAM1, param1);
            args.PutString(ARG_PARAM2, param2);
            fragment.Arguments = args;
            return fragment;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            if (Arguments != null)
            {
                _param1 = Arguments.GetString(ARG_PARAM1);
                _param2 = Arguments.GetString(ARG_PARAM2);
            }
            _clsName = this.Class.SimpleName + ":";
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container,
                            Bundle savedInstanceState)
        {
            // Inflate the layout for this fragment
            return inflater.Inflate(Resource.Layout.fragment_base, container, false);
        }

        public void OnButtonPressed(Uri uri)
        {
            if (_listener != null)
            {
                _listener.OnFragmentInteraction(uri);
            }
        }

        public override void OnAttach(Context context)
        {
            base.OnAttach(context);


            if (context is IOnFragmentInteractionListener) {
                _listener = (IOnFragmentInteractionListener)context;
            } else
            {
                throw new RuntimeException(context.ToString()
                        + " must implement OnFragmentInteractionListener");
            }
        }

        public override void OnDetach()
        {
            base.OnDetach();
            _listener = null;
        }
    }
}