using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace WpfApp3.Plugins
{
    public partial class HtmlEditorControl : UserControl
    {
        private string _currentFilePath;
        private bool _isModified;

        public HtmlEditorControl()
        {
            InitializeComponent();
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "HTML Files (*.html;*.htm)|*.html;*.htm|SVG Files (*.svg)|*.svg|All Files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    _currentFilePath = openFileDialog.FileName;
                    EditorBox.Text = File.ReadAllText(_currentFilePath);
                    _isModified = false;
                    UpdateStatus();
                    UpdatePreview();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error opening file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_currentFilePath))
            {
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "HTML Files (*.html)|*.html|SVG Files (*.svg)|*.svg|All Files (*.*)|*.*"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    _currentFilePath = saveFileDialog.FileName;
                }
                else
                {
                    return;
                }
            }

            try
            {
                File.WriteAllText(_currentFilePath, EditorBox.Text);
                _isModified = false;
                UpdateStatus();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            UpdatePreview();
        }

        private void EditorBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isModified)
            {
                _isModified = true;
                UpdateStatus();
            }
        }

        private void UpdateStatus()
        {
            var status = string.IsNullOrEmpty(_currentFilePath) ? "New File" : Path.GetFileName(_currentFilePath);
            if (_isModified) status += "*";
            StatusText.Text = status;
        }

        private void UpdatePreview()
        {
            try
            {
                var content = EditorBox.Text;
                if (string.IsNullOrWhiteSpace(content))
                {
                    PreviewBrowser.NavigateToString("<html><body></body></html>");
                    return;
                }

                // If content looks like SVG but not full HTML, wrap it? 
                // WebBrowser can handle SVG if it has proper XML headers, strictly speaking.
                // But usually wrapping in HTML body is safer for quick preview if it's just a snippet.
                // However, let's just try NavigateToString which accepts HTML string.
                // If it's a pure SVG file content, modern IE/Edge engine inside WebBrowser control might handle it or not depending on registry settings.
                // To be safe for SVG, we can wrap it in a simple HTML container.
                
                if (content.TrimStart().StartsWith("<svg", StringComparison.OrdinalIgnoreCase) || 
                    (content.Contains("<svg") && !content.Contains("<html")))
                {
                    content = $"<html><body>{content}</body></html>";
                }

                PreviewBrowser.NavigateToString(content);
            }
            catch (Exception ex)
            {
               // System.Diagnostics.Debug.WriteLine($"Preview error: {ex.Message}");
            }
        }
    }
}
