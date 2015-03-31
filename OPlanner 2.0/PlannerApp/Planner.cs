using PlannerNameSpace.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace PlannerNameSpace
{
    public partial class Planner :DispatcherObject
    {
        public static bool IsShuttingDown = false;
        public UserPreferences UserPreferences;
        private ItemRepository m_itemRepository;
        private static readonly Planner m_instance = new Planner();
        public AsyncObservableCollection<string> AdminAliases { get; set; }
        public AsyncObservableCollection<string> DevAliases { get; set; }
        private AsyncObservableCollection<string> m_eventLog;

        public static Planner Instance
        {
            get { return m_instance; }
        }

        public ItemRepository ItemRepository
        {
            get
            {
                return m_itemRepository;
            }
        }

        private Planner()
        {
            int foo1 = 17;
            int foo2 = 22;
            int foo3 = 913;
            int foo4 = 22;

            int[] fileValues = new int[1001];

            fileValues[foo1]++;
            fileValues[foo2]++;
            fileValues[foo3]++;
            fileValues[foo4]++;

            CurrentUserAlias = GetCurrentUserName().ToLower(System.Globalization.CultureInfo.CurrentCulture);
            IsStartupComplete = false;

            AdminAliases = new AsyncObservableCollection<string>();
            AdminAliases.Add("tracef");
            AdminAliases.Add("dconger");
            AdminAliases.Add("imranazi");
            AdminAliases.Add("scottmcf");

            DevAliases = new AsyncObservableCollection<string>();
            DevAliases.Add("tracef");

            m_eventLog = new AsyncObservableCollection<string>();
        }

        MainWindow m_mainWindow;

        public MainWindow MainWindow
        {
            get { return m_mainWindow; }
            set { m_mainWindow = value; }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Main Planner entry point
        /// </summary>
        //------------------------------------------------------------------------------------
        public void Startup(App app)
        {
            // We're going to put up a start-up dialog before showing the main window, so set 
            // ShutdownMode so the app doesn't close when the start-up dialog is taken down.
            app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            ToolTipService.ShowDurationProperty.OverrideMetadata(typeof(DependencyObject), new FrameworkPropertyMetadata(Int32.MaxValue));

            bool shouldClearProductGroup;
            if (InitializeStartupOptions(out shouldClearProductGroup))
            {
                m_itemRepository = new ItemRepository();
                m_itemRepository.InitializeRepository(shouldClearProductGroup);

                if (!IsShuttingDown)
                {
                    IsStartupComplete = true;
                    UserPreferences.SetGlobalPreference(GlobalPreference.LastOpenProductGroupKey, Planner.Instance.CurrentProductGroupKey);

                    // Now re-set ShutdownMode so that the app will close normally when the main window is closed.
                    app.ShutdownMode = System.Windows.ShutdownMode.OnLastWindowClose;

                    MainWindow = new MainWindow();
                    MainWindow.Show();

                    if (ApplicationStartupComplete != null)
                    {
                        ApplicationStartupComplete(this, EventArgs.Empty);
                    }
                }
            }
        }

        public static void Shutdown()
        {
            IsShuttingDown = true;
            App.Current.Shutdown();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Restarts the application with the last-opened product group set to the given
        /// product key.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static void Restart(string newProductGroupKey)
        {
            Planner.Instance.UserPreferences.SetGlobalPreference(GlobalPreference.LastOpenProductGroupKey, newProductGroupKey);
            Process.Start(System.Windows.Forms.Application.ExecutablePath);
            Shutdown();
        }


        //------------------------------------------------------------------------------------
        /// <summary>
        /// Give user diagnostic start-up options, initialize user preferences, etc.
        /// </summary>
        //------------------------------------------------------------------------------------
        bool InitializeStartupOptions(out bool shouldClearProductGroup)
        {
            bool optionsDialogInUse = false;
            bool shouldUseCloneHostStore = false;
            bool shouldClearUserPreferences = false;
            shouldClearProductGroup = false;
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.LeftShift))
            {
                StartupOptionsDialog dialog = new StartupOptionsDialog();
                dialog.ShowDialog();

                if (dialog.Result == DialogResult.Quit)
                {
                    Shutdown();
                    return false;
                }

                else if (dialog.Result != DialogResult.Cancel)
                {
                    optionsDialogInUse = true;
                    shouldUseCloneHostStore = dialog.ShouldUseClone;
                    shouldClearUserPreferences = dialog.ShouldClearUserPreferences;
                    shouldClearProductGroup = dialog.ShouldClearCurrentProductGroup;
                }
            }

            UserPreferences = UserPreferences.Unserialize(shouldClearUserPreferences);

            if (optionsDialogInUse)
            {
                UserPreferences.ShouldUseCloneHostStore = shouldUseCloneHostStore;
            }

            if (shouldUseCloneHostStore)
            {
                shouldClearProductGroup = true;
            }

            return true;
        }

        public string CurrentUserAlias { get; set; }
        public bool IsStartupComplete { get; set; }

        public static string ProductGroupName
        {
            get
            {
                string productGroupName = Constants.c_None;
                ProductGroupItem currentProduct = Planner.Instance.CurrentProductGroup;
                if (currentProduct != null)
                {
                    productGroupName = currentProduct.Title;
                    if (Planner.Instance.UserPreferences.ShouldUseCloneHostStore)
                    {
                        productGroupName += " (Clone)";
                    }
                }

                return productGroupName;
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// This is the default 'Team' name to set in order to retrieve the subset of specs 
        /// from the backing store owned by this product group.
        /// </summary>
        //------------------------------------------------------------------------------------
        public string DefaultSpecTeamName
        {
            get
            {
                if (CurrentProductGroup != null)
                {
                    return CurrentProductGroup.DefaultSpecTeamName;
                }

                return Constants.c_NoneSpecTeamName;
            }

            set
            {
                if (CurrentProductGroup != null)
                {
                    CurrentProductGroup.DefaultSpecTeamName = value;
                    if (DefaultSpecTeamNameChanged != null)
                    {
                        DefaultSpecTeamNameChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns a list of the aliases for all the members of the current product group.
        /// </summary>
        //------------------------------------------------------------------------------------
        public List<string> GetMemberAliases()
        {
            if (CurrentProductGroup != null)
            {
                return CurrentProductGroup.MemberAliases;
            }

            return new List<string>();
        }

        public string CurrentProductGroupKey
        {
            get
            {
                return m_itemRepository.GetCurrentProductGroupKey();
            }
        }

        public ProductGroupItem CurrentProductGroup
        {
            get
            {
                return m_itemRepository.GetCurrentProductGroup();
            }
        }

        public DateTime LastRefreshTime { get; set; }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// GetCurrentUserName
        /// 
        /// Gets the alias (minus domain) of the current user running the app.
        /// </summary>
        //------------------------------------------------------------------------------------
        private string GetCurrentUserName()
        {
            string currentUser = null;
            string domainName = GetCurrentUserDomainName();
            if (!String.IsNullOrEmpty(domainName))
            {
                int userIndex = domainName.LastIndexOf('\\');
                if (userIndex >= 0)
                {
                    currentUser = domainName.Substring(userIndex + 1);
                }
            }

            return currentUser;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// GetCurrentUserDomainName
        /// 
        /// Gets the domain fieldName of the user currently running the app.
        /// </summary>
        //------------------------------------------------------------------------------------
        private string GetCurrentUserDomainName()
        {
            return System.Security.Principal.WindowsIdentity.GetCurrent().Name;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the application ProductName FieldName.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public bool IsUserOPlannerDev(string alias)
        {
            return DevAliases.Contains(alias);
        }

        public bool IsCurrentUserOPlannerDev()
        {
            return IsUserOPlannerDev(CurrentUserAlias);
        }

        public bool IsUserAdmin(string alias)
        {
            return AdminAliases.Contains(alias);
        }

        public bool IsCurrentUserAdmin()
        {
            return IsUserAdmin(CurrentUserAlias);
        }

        public bool ConfirmIsAdmin(string featureRequiringAdmin)
        {
            if (!IsCurrentUserAdmin())
            {
                //AdminRequiredDialog dialog = new AdminRequiredDialog(featureRequiringAdmin);
                //dialog.ShowDialog();

                return false;
            }

            return true;
        }

        public AsyncObservableCollection<string> EventLogEntries { get { return m_eventLog; } }

        public void ClearEventLog()
        {
            m_eventLog.Clear();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Adds the given text to the event log
        /// </summary>
        //------------------------------------------------------------------------------------
        public void WriteToEventLog(string text)
        {
            try
            {
                if (!text.EndsWith("."))
                    text += ".";

                string entry = DateTime.Now.ToLongTimeString() + ": " + text;
                m_eventLog.Add(entry);
            }
            catch (Exception) { }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Add information about the given item to the event log, along with the given 
        /// message.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void AddStoreItemLogEntry(StoreItem item, string message)
        {
            WriteToEventLog(" ID=" + item.ID.ToString() + "; Title=" + item.Title + "; " + message);
        }

        string CurrentWarning = null;
        public string GetCurrentWarning()
        {
            return CurrentWarning;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Add information about the given item to the event log, along with the given 
        /// message.  Returns true if the exceptions was successfully handled.
        /// </summary>
        //------------------------------------------------------------------------------------
        public bool HandleException(Exception exception)
        {
            WriteToEventLog(exception.Message);
            WriteToEventLog(exception.StackTrace);
            return false;
        }

        public void HandleStoreItemException(StoreItem item, Exception exception)
        {
            WriteToEventLog(exception.Message);
            WriteToEventLog(exception.StackTrace);

            if (item.CommitErrorState == CommitErrorStates.ErrorAccessingAttachmentShare)
            {
                CurrentWarning = Constants.ProductStudioMiddleTierProblems;
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Displays an appropriate error message based on the given error result.
        /// </summary>
        //------------------------------------------------------------------------------------
        public void HandleFailedTask(BackgroundTaskResult result)
        {
            StoreErrors error = Datastore.GetStoreErrorFromExceptionMessage(result.ResultMessage);
            if (error == StoreErrors.ProductStudioNotInstalled || error == StoreErrors.ProductStudioNewerVersionRequired)
            {
                //ProductStudioNotInstalledDialog dialog = new ProductStudioNotInstalledDialog(error);
                //dialog.ShowDialog();
            }
            else
            {
                UserMessage.Show(result.ResultMessage);
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the full path to the root storage folder for this application.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static string GetOPlannerFolder()
        {
            string appPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            appPath = Path.Combine(appPath, RootProductName);
            if (!Directory.Exists(appPath))
            {
                Directory.CreateDirectory(appPath);
            }

            return appPath;
        }

        public static string RootProductName
        {
            get { return "OPlanner20"; }
        }

        static BitmapSource GenericProfileSource = null;
        public BitmapSource GenericProfileBitmap
        {
            get
            {
                if (GenericProfileSource == null)
                {
                    var hBitmap = Properties.Resources.GenericProfile.GetHbitmap();
                    GenericProfileSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                }

                return GenericProfileSource;
            }
        }

    }
}
