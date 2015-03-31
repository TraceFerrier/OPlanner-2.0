using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace PlannerNameSpace.View.Content
{
    public partial class PlannerRichTextControl : UserControl
    {
        bool m_documentLoading = false;
        public PlannerRichTextControl()
        {
            InitializeComponent();
            PlannerRichTextBox.AddHandler(Hyperlink.RequestNavigateEvent, new RequestNavigateEventHandler(HandleRequestNavigate));
            PlannerRichTextBox.SelectionChanged += PlannerRichTextBox_SelectionChanged;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Will be called each time the store item bound to this rich text control changes.
        /// </summary>
        //------------------------------------------------------------------------------------
        private static async void OnSourceStoreItemChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            PlannerRichTextControl richTextControl = (PlannerRichTextControl)dependencyObject;
            RichTextBox richTextBox = richTextControl.PlannerRichTextBox;

            if (e.NewValue != null)
            {
                StoreItem storeItem = (StoreItem)e.NewValue;

                if (!richTextControl.m_documentLoading)
                {
                    richTextControl.m_documentLoading = true;
                    richTextBox.TextChanged -= richTextControl.PlannerRichTextBox_TextChanged;
                    SetRichTextBoxText(richTextBox, "<Loading..>");
                    MemoryStream document = await storeItem.GetDocumentValueAsync(richTextControl.SourceStoreItemPropName);
                    SetRichTextBoxDocument(richTextBox, document);
                    richTextControl.PlannerRichTextBox.TextChanged += richTextControl.PlannerRichTextBox_TextChanged;
                    richTextControl.m_documentLoading = false;
                }
            }

        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Save changes in memory (and dirty the current store item) each time the user
        /// updates the rich text.
        /// </summary>
        //------------------------------------------------------------------------------------
        void PlannerRichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!m_documentLoading)
            {
                SourceStoreItem.SaveDocumentFromRichTextBox(SourceStoreItemPropName, PlannerRichTextBox);
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Utility function to set the given document stream as the contents of the given
        /// RichTextBox.
        /// </summary>
        //------------------------------------------------------------------------------------
        private static void SetRichTextBoxDocument(RichTextBox richTextBox, MemoryStream document)
        {
            if (document != null)
            {
                TextRange range = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
                range.Load(document, DataFormats.Rtf);
            }
            else
            {
                SetRichTextBoxText(richTextBox, "");
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Utility function to set the given text as the contents of the given RichTextBox.
        /// </summary>
        //------------------------------------------------------------------------------------
        static void SetRichTextBoxText(RichTextBox richTextBox, string text)
        {
            TextRange range = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
            range.Text = text;
        }

        public static readonly DependencyProperty HeaderTextProperty = DependencyProperty.Register(
            "HeaderText", typeof(string), typeof(PlannerRichTextControl),
            new FrameworkPropertyMetadata());

        public static readonly DependencyProperty SourceStoreItemPropNameProperty = DependencyProperty.Register(
            "SourceStoreItemPropName", typeof(string), typeof(PlannerRichTextControl),
            new FrameworkPropertyMetadata());

        public static readonly DependencyProperty SourceStoreItemProperty = DependencyProperty.Register(
            "SourceStoreItem", typeof(StoreItem), typeof(PlannerRichTextControl),
            new FrameworkPropertyMetadata(OnSourceStoreItemChanged));

        public string HeaderText
        {
            get { return (string)GetValue(HeaderTextProperty); }
            set { SetValue(HeaderTextProperty, value); }
        }

        public string SourceStoreItemPropName
        {
            get { return (string)GetValue(SourceStoreItemPropNameProperty); }
            set { SetValue(SourceStoreItemPropNameProperty, value); }
        }

        public StoreItem SourceStoreItem
        {
            get 
            {
                object item = GetValue(SourceStoreItemProperty);
                StoreItem storeItem = item as StoreItem;
                return storeItem;
            }
            set 
            {
                SetValue(SourceStoreItemProperty, value); 
            }
        }

        public Window ParentWindow { get; set; }

        private static void HandleRequestNavigate(object sender, RequestNavigateEventArgs args)
        {
            string url = args.Uri.ToString();

            try
            {
                Process.Start(url);
            }

            catch (Exception)
            {
                UserMessage.Show("Unable to navigate to the given URL: " + url);
            }

        }

        void PlannerRichTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            TextRange range = new TextRange(PlannerRichTextBox.Selection.Start, PlannerRichTextBox.Selection.End);
            CreateHyperlinkButton.IsEnabled = !range.IsEmpty;
        }

        private void CreateHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            //RichTextBox richTextBox = PlannerRichTextBox;
            //TextRange range = new TextRange(richTextBox.Selection.Start, richTextBox.Selection.End);

            //string hyperlinkText = null;
            //if (!range.IsEmpty)
            //{
            //    hyperlinkText = range.Text;
            //}

            //EditHyperlink dialog = new EditHyperlink(ParentWindow, hyperlinkText);
            //dialog.ShowDialog();
            //if (!dialog.IsCancelled)
            //{
            //    Hyperlink link = new Hyperlink(range.Start, range.End);
            //    link.NavigateUri = new System.Uri(dialog.HyperlinkAddress);
            //}
        }

        private void UndockButton_Click(object sender, RoutedEventArgs e)
        {
            //RichTextEditDialog dialog = new RichTextEditDialog(RichTextContext.CurrentItemEditorWindow, PlannerRichTextBox);
            //dialog.Closed += dialog_Closed;
            //dialog.ShowDialog();
        }

        void dialog_Closed(object sender, EventArgs e)
        {
            //RichTextEditDialog dialog = sender as RichTextEditDialog;

            //TextRange range = new TextRange(dialog.DialogRichTextBox.Document.ContentStart, dialog.DialogRichTextBox.Document.ContentEnd);
            //MemoryStream mStream = new MemoryStream();
            //range.Save(mStream, DataFormats.Rtf);

            //TextRange newRange = new TextRange(PlannerRichTextBox.Document.ContentStart, PlannerRichTextBox.Document.ContentEnd);
            //newRange.Load(mStream, DataFormats.Rtf);

        }
    }
}
