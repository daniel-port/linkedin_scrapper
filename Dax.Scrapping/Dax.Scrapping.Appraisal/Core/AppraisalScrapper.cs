using CefSharp;
using CefSharp.WinForms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dax.Scrapping.Appraisal.Core
{
    public class AppraisalScrapper : ScrapperBase, IDisposable
    {
        #region fields

        private Action _curAction = null;

        public delegate void LogMsg(string msg);
        public event LogMsg OnLog = null;

        public delegate void Completed();
        public event Completed OnCompleted = null;


        private Status _curStatus = Status.Paused;
        string _user;
        string _pass;
        string _loginUrl = "https://www.mytitlesourceconnection.com/Account/Login";
        string _newOrdersUrl = "https://www.mytitlesourceconnection.com/Vendor/AppraiserQueue/NewOrders";



        #endregion

        #region constructors
        public AppraisalScrapper(string user, string pass)
        {
            _user = user;
            _pass = pass;

            _brouserComponent = new ChromiumWebBrowser(_loginUrl);
            _brouserComponent.LoadingStateChanged += _brouserComponent_LoadingStateChanged;
            _curStatus = Status.Loading;

        }
        #endregion

        #region Properties
        #endregion

        #region Public


        #endregion

        #region helpers
        private void LoginUser()
        {
            var scriptTmpl = @"(function() {                                        
                                    var elemUser = document.getElementById('UserName');
                                    elemUser.value = '{0}';
                                    var elemPass = document.getElementById('Password');
                                    elemPass.value = '{1}';
                                    document.getElementsByClassName('btn-login')[0].click();
                               })();";

            var script = scriptTmpl.Replace("{0}", _user).Replace("{1}", _pass);
            _brouserComponent.ExecuteScriptAsync(script);
        }

        private void Log(string msg)
        {
            if (OnLog != null)
                OnLog(msg);
        }


        void _brouserComponent_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (!e.IsLoading)
            {

                switch (_curStatus)
                {
                    case Status.Loading:
                        _curStatus = Status.Loggin;
                        Log("Loging user..");
                        LoginUser();
                        break;

                    case Status.Loggin:
                        _curStatus = Status.Searching;
                        Log("Going to orders page....");
                        GoToOrdersPage();
                        break;

                    case Status.Searching:
                        _curStatus = Status.GetOrders;
                        Log("Getting Orders....");
                        AcceptOrders();
                        break;

                    case Status.GetOrders:
                        // GetOrders();
                        break;

                    case Status.Completed:
                        _curStatus = Status.Completed;
                        Log("Completed....");
                        if (OnCompleted != null)
                            OnCompleted();
                        break;

                }
               /// Log(Helper.GetEnumDescription<Status>(_curStatus));

            }
            else
            {
                // StopLoadingInProgress();
            }
        }

        private void AcceptOrders()
        {
            var scriptTmplClickOrder = @"(function() {
                                   var ordersList = document.getElementsByClassName('open-AcceptOrderDialog');
                                   var result = 0;
                                   if(ordersList && ordersList.length > 0)
                                    {
                                       ordersList[0].click();
                                       result =  ordersList.length;
                                    }

                                    return result;
                               })();";


            _brouserComponent.EvaluateScriptAsync(scriptTmplClickOrder).ContinueWith(t =>
            {
                if (!t.IsFaulted)
                {
                    var resultString = t.Result.Result.ToString();
                    var intResult = Convert.ToInt32(resultString);
                    Log("Result getting orders from list...." + resultString);
                    if (intResult > 0)
                    {
                        ClickModal();
                        var pending = intResult - 1;
                        if (pending > 0)
                        {
                            _curStatus = Status.Searching;
                            GoToOrdersPage();
                            return;
                        }
                    }

                    if (OnCompleted != null)
                    {
                        OnCompleted();
                    }

                }
                else
                {
                    Log("error  invoking javascript..." + t.Result.Message);
                    if (OnCompleted != null)
                    {
                        OnCompleted();
                    }
                }
            });
        }

        private void ClickModal()
        {
            Log("clicking modal.....");
            //ensure modal is shown
            Thread.Sleep(2000);

            var scriptTmplAcceptOrder = @"(function() {
                                           var dialogAcceptButtons = document.getElementsByName('btnAccept');
                                           if(dialogAcceptButtons && dialogAcceptButtons.length > 0)
                                            {
                                                for(i = 0, i < dialogAcceptButtons.length; i++)
                                                { 
                                                    try
                                                    { 
                                                         var attrFlag = dialogAcceptButtons[i].getAttribute('data-order-detail-id');
                                                         if(attrFlag)
                                                         {
                                                               dialogAcceptButtons[i].click(); 
                                                         }
                                                    }
                                                    catch(err)
                                                    {

                                                    }
                                                }
                                            }

                                       })();";

            _brouserComponent.ExecuteScriptAsync(scriptTmplAcceptOrder);
            //ensure order is accepted
            Log("Modal Clicked.....");
            Thread.Sleep(2000);
        }

        private void GetOrders()
        {
            var scriptTmpl = @"(function() {         
                                     
                                      var result = '', OrderDetailId = '', ProductType = '', TransactionType='', OrderType= '', DueDate = '', ClientLoanNumber = '', Feed = '', Address = '', City = '', Country = '', State = '', Zip = '', Client='', AssignedDate = '';                               
                                      try {OrderDetailId =  document.getElementById('name').getElementsByClassName('full-name')[0].innerText; }catch(err){};
                                      try {ProductType = document.getElementById('headline').getElementsByTagName('p')[0].innerText; }catch(err){};
                                      try {TransactionType = document.getElementById('overview-summary-current').getElementsByTagName('a')[2].innerText; }catch(err){};
                                      try {OrderType = document.getElementById('overview-summary-past').getElementsByTagName('a')[2].innerText; }catch(err){};    
                                      try {DueDate = document.getElementById('overview-summary-DueDate').getElementsByTagName('a')[2].innerText; }catch(err){};
                                      try {ClientLoanNumber = document.getElementById('ClientLoanNumber-view').getElementsByTagName('a')[0].innerText; }catch(err){};
                                      try {Feed = document.getElementsByClassName('view-public-profile')[0].innerText; }catch(err){};
                                      try {Address = document.getElementById('Address').getElementsByTagName('a')[0].innerText;}catch(err){};
                                      try {City = document.getElementById('Address').getElementsByTagName('a')[0].innerText;}catch(err){};
                                      try {Country = document.getElementById('Address').getElementsByTagName('a')[0].innerText;}catch(err){};
                                      try {State = document.getElementById('Address').getElementsByTagName('a')[0].innerText;}catch(err){};
                                      try {Zip = document.getElementById('Address').getElementsByTagName('a')[0].innerText;}catch(err){};
                                      try {Client = document.getElementById('Address').getElementsByTagName('a')[0].innerText;}catch(err){};
                                      try {AssignedDate = document.getElementById('Address').getElementsByTagName('a')[0].innerText;}catch(err){};

								 
                                    result = { 'OrderDetailId': OrderDetailId, 'TransactionType':TransactionType, 'OrderType':OrderType, 'ProductType':ProductType, 'DueDate':DueDate, 'ClientLoanNumber':ClientLoanNumber, 'Address':Address, 'Feed':Feed, 'City':City, 'Country':Country, 'State':State, 'Zip':Zip, 'Client':Client, 'AssignedDate': AssignedDate} 

                                    return JSON.stringify(result);
                                })();";

            var script = scriptTmpl;
            _brouserComponent.EvaluateScriptAsync(script).ContinueWith(t =>
            {
                if (!t.IsFaulted)
                {
                    var curContactString = t.Result.Result.ToString();
                    var curContactObject = JsonConvert.DeserializeObject<Order>(curContactString);

                }
            });
        }

        private void GoToOrdersPage()
        {

            _brouserComponent.Load(_newOrdersUrl);
        }



        private void SaveData()
        {
            //try
            //{
            //    if (!string.IsNullOrEmpty(this.txtPath.Text))
            //    {
            //        Helper.WriteCSV<Order>(_ContactList, this.txtPath.Text);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
        }

        public void Dispose()
        {

        }
        #endregion
    }
}
