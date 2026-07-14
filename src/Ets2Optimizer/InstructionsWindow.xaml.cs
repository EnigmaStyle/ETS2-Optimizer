using System.Windows;

namespace Ets2Optimizer;

public partial class InstructionsWindow : Window
{
    public InstructionsWindow()
    {
        InitializeComponent();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
