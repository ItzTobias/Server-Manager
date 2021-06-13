using Server_Manager.Scripts;
using ServerManagerFramework;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Server_Manager.Views
{
    /// <summary>
    /// Interaction logic for ServerInfo.xaml
    /// </summary>
    public partial class ServerInfo : Grid, IHasTopMenuItems
    {
        public UIElement[] Items { get; } = new UIElement[1]
        {
            new TextBlock()
            {
                Style = Application.Current.Resources["Header"] as Style,
                Foreground = SMR.WhiteBrush,
                Text = "Edit Server"
            }
        };

        public IHasDirectory IHasDirectory { get; }

        public string ServerName { get; set; }

        public ServerInfo(IHasDirectory iHasDirectory)
        {
            InitializeComponent();

            IHasDirectory = iHasDirectory;
            ServerName = Path.GetFileName(IHasDirectory.Directory);

            if (IHasDirectory is IServer server)
            {
                StartStopButton.IServer = server;
                StartStopButton.Visibility = Visibility.Visible;
            }

            if (IHasDirectory is ICommandLineOutput output)
            {
                Terminal.Visibility = Visibility.Visible;
                Output.Visibility = Visibility.Visible;
                TopbarButtons.Visibility = Visibility.Visible;
                ClearButton.Visibility = Visibility.Visible;

                ConsoleItemsControl.ItemsSource = output.ConsoleLines;

                if (output.ConsoleLines is ObservableCollection<CommandLine> observableCommandLine)
                {
                    observableCommandLine.CollectionChanged += ConsoleLineAdded;
                }
            }

            if (IHasDirectory is ICommandLineInput input)
            {
                Terminal.Visibility = Visibility.Visible;
                Input.Visibility = Visibility.Visible;
                TopbarButtons.Visibility = Visibility.Visible;
            }
        }


        private void Clear(object sender, RoutedEventArgs e)
        {
            ICommandLineOutput output = IHasDirectory as ICommandLineOutput;
            output.ClearLines();
        }
        private void ConsoleLineAdded(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (ConsoleScrollViewer.VerticalOffset > ConsoleScrollViewer.ScrollableHeight - 10)
            {
                ConsoleScrollViewer.ScrollToEnd();
            }
        }
        private void CommandLine_TextChanged(object sender, TextChangedEventArgs e)
        {
        }
        private void SendCommand(object sender, RoutedEventArgs e)
        {
            ICommandLineInput input = IHasDirectory as ICommandLineInput;
            input.WriteLine(CommandLine.Text);

            if (IHasDirectory is ICommandLineOutput output)
            {
                if (string.IsNullOrEmpty(CommandLine.Text))
                {
                    CommandLine.Text = " ";
                }

                output.AddLine(new CommandLine()
                {
                    Message = CommandLine.Text,
                    FontStyle = System.Windows.Media.StyleSimulations.BoldSimulation,
                    FontColor = SMR.SWhiteBrush
                });
            }

            CommandLine.Text = "";
        }

        private void OnSaveClick(object sender, RoutedEventArgs e)
        {
            SaveName();
        }
        private void SaveName()
        {
            if (ServerName == Path.GetFileName(IHasDirectory.Directory))
            {
                return;
            }

            string newDirectory = Path.Combine(GlobalConfig.ManagerPath, ServerName);

            if (Directory.Exists(newDirectory))
            {
                return;
            }

            Directory.Move(IHasDirectory.Directory, newDirectory);

            typeof(IHasDirectory)
                .GetProperty(nameof(IHasDirectory.Directory))
                .SetValue(IHasDirectory, newDirectory);
        }

        private void OnBackClick(object sender, RoutedEventArgs e)
        {
            _ = MainWindow.GetMainWindow.ChangeCurrentControl(new ServerList());
        }
    }
}
/*
    <ScrollViewer Grid.Row="2"
                  SnapsToDevicePixels="True"
                  Margin="16, 0, 0, 0">
        <StackPanel Margin="0, 20"
                    Width="400">

            <Grid Margin="0, 0, 0, 25">
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="*" />
                    <ColumnDefinition Width="20" />
                    <ColumnDefinition Width="48" />
                </Grid.ColumnDefinitions>

                <StackPanel>
                    <TextBlock Foreground="White"
                               DockPanel.Dock="Top"
                               FontSize="11"
                               FontWeight="Bold"
                               Text="NAME"
                               Margin="0, 0, 0, 3" />
                    <TextBox Style="{StaticResource BlackTextBox}"
                             DockPanel.Dock="Top"
                             GotKeyboardFocus="SelectAllText"
                             GotMouseCapture="SelectAllText"
                             x:Name="ServerName" />
                </StackPanel>
            </Grid>

            <Expander Header="CONSOLE"
                      Style="{StaticResource ServerInfoExpander}"
                      Margin="0, 0, 0, 25">
                <StackPanel>
                    <Grid Height="30"
                          Margin="0, 5, 0, 0">
                        <Grid.ColumnDefinitions>
                            <ColumnDefinition Width="*" />
                            <ColumnDefinition Width="5" />
                            <ColumnDefinition Width="*" />
                            <ColumnDefinition Width="5" />
                            <ColumnDefinition Width="*" />
                        </Grid.ColumnDefinitions>

                        <ToggleButton Grid.Column="0"
                                      Style="{StaticResource ConsoleToggle}"
                                      Checked="ToggleTimeVisibilityOn"
                                      Unchecked="ToggleTimeVisibilityOff">
                            <TextBlock Text="Show Time"
                                       VerticalAlignment="Center"
                                       TextAlignment="Center" />
                        </ToggleButton>
                        <ToggleButton Grid.Column="2"
                                      Style="{StaticResource ConsoleToggle}"
                                      Checked="ToggleThreadVisibilityOn"
                                      Unchecked="ToggleThreadVisibilityOff">
                            <TextBlock Text="Show Thread"
                                       VerticalAlignment="Center"
                                       TextAlignment="Center" />
                        </ToggleButton>
                        <Button Grid.Column="4"
                                Click="ClearConsoleOutput"
                                Style="{StaticResource BlackButton}">
                            <TextBlock Text="Clear"
                                       VerticalAlignment="Center"
                                       TextAlignment="Center" />
                        </Button>
                    </Grid>

                    <Border CornerRadius="4"
                            BorderBrush="{StaticResource Input}"
                            Background="{StaticResource Input}"
                            Margin="0, 5, 0, 0">
                        <ScrollViewer VerticalScrollBarVisibility="Auto"
                                      Height="250"
                                      Margin="8, 5, 5, 5"
                                      x:Name="ConsoleScrollViewer"
                                      DataContextChanged="ConsoleLineAdded">
                            <ItemsControl x:Name="ConsoleItemsControl">
                                <ItemsControl.ItemTemplate>
                                    <DataTemplate>
                                        <Glyphs FontUri="/Fonts/NotoSans-hinted/NotoSans-Regular.ttf"
                                                FontRenderingEmSize="12"
                                                UnicodeString="{Binding Data}"
                                                Fill="{Binding Color}"
                                                OriginY="12" />
                                    </DataTemplate>
                                </ItemsControl.ItemTemplate>
                            </ItemsControl>
                        </ScrollViewer>
                    </Border>

                    <Grid Margin="0, 5, 0, 0">
                        <Grid.ColumnDefinitions>
                            <ColumnDefinition Width="*" />
                            <ColumnDefinition Width="5" />
                            <ColumnDefinition Width="30" />
                        </Grid.ColumnDefinitions>

                        <TextBox Style="{StaticResource BlackTextBox}"
                                 x:Name="CommandLine"
                                 TextChanged="CommandLine_TextChanged" />
                    </Grid>
                </StackPanel>
            </Expander>

        </StackPanel>
    </ScrollViewer>

        <Style x:Key="ExpanderDownHeaderStyle"
               TargetType="{x:Type ToggleButton}">
            <Setter Property="Background"
                    Value="Transparent" />
            <Setter Property="Template">
                <Setter.Value>
                    <ControlTemplate TargetType="{x:Type ToggleButton}">
                        <Border Padding="{TemplateBinding Padding}"
                                Background="{TemplateBinding Background}"
                                BorderThickness="1"
                                CornerRadius="4">
                            <Grid Margin="3, 0">
                                <Grid.ColumnDefinitions>
                                    <ColumnDefinition Width="Auto" />
                                    <ColumnDefinition Width="11" />
                                </Grid.ColumnDefinitions>

                                <ContentPresenter Grid.Column="0"
                                                  RecognizesAccessKey="True"
                                                  HorizontalAlignment="Left"
                                                  VerticalAlignment="Center"
                                                  Margin="0, 0, 5, 0" />

                                <Image Grid.Column="1"
                                       x:Name="arrow"
                                       Source="Images/Arrows/arrow_down.png"
                                       HorizontalAlignment="Center"
                                       VerticalAlignment="Center"
                                       Width="11" />

                            </Grid>
                        </Border>
                        <ControlTemplate.Triggers>
                            <Trigger Property="IsMouseOver"
                                     Value="True">
                                <Setter Property="Source"
                                        TargetName="arrow"
                                        Value="Images/Arrows/arrow_down_hover.png" />
                                <Setter Property="Background"
                                        Value="#1AFFFFFF" />
                            </Trigger>
                        </ControlTemplate.Triggers>
                    </ControlTemplate>
                </Setter.Value>
            </Setter>
        </Style>
        <Style x:Key="ExpanderUpHeaderStyle"
               TargetType="{x:Type ToggleButton}">
            <Setter Property="Background"
                    Value="Transparent" />
            <Setter Property="Template">
                <Setter.Value>
                    <ControlTemplate TargetType="{x:Type ToggleButton}">
                        <Border Padding="{TemplateBinding Padding}"
                                Background="{TemplateBinding Background}"
                                BorderThickness="1"
                                CornerRadius="4">
                            <Grid Margin="3, 0">
                                <Grid.ColumnDefinitions>
                                    <ColumnDefinition Width="Auto" />
                                    <ColumnDefinition Width="11" />
                                </Grid.ColumnDefinitions>

                                <ContentPresenter Grid.Column="0"
                                                  RecognizesAccessKey="True"
                                                  HorizontalAlignment="Left"
                                                  VerticalAlignment="Center"
                                                  Margin="0, 0, 5, 0" />

                                <Image Grid.Column="1"
                                       x:Name="arrow"
                                       Source="Images/Arrows/arrow_up.png"
                                       HorizontalAlignment="Center"
                                       VerticalAlignment="Center"
                                       Width="11" />

                            </Grid>
                        </Border>
                        <ControlTemplate.Triggers>
                            <Trigger Property="IsMouseOver"
                                     Value="True">
                                <Setter Property="Source"
                                        TargetName="arrow"
                                        Value="Images/Arrows/arrow_up_hover.png" />
                                <Setter Property="Background"
                                        Value="#1AFFFFFF" />
                            </Trigger>
                        </ControlTemplate.Triggers>
                    </ControlTemplate>
                </Setter.Value>
            </Setter>
        </Style>

        <Style x:Key="ServerInfoExpander"
               TargetType="{x:Type Expander}">
            <Setter Property="Template">
                <Setter.Value>
                    <ControlTemplate TargetType="{x:Type Expander}">
                        <Border BorderBrush="{TemplateBinding BorderBrush}"
                                BorderThickness="0">
                            <StackPanel>
                                <ToggleButton x:Name="HeaderSite"
                                              IsChecked="{Binding IsExpanded, Mode=TwoWay, RelativeSource={RelativeSource TemplatedParent}}"
                                              Style="{StaticResource ExpanderDownHeaderStyle}"
                                              Content="{TemplateBinding Header}"
                                              HorizontalAlignment="Right"
                                              Foreground="White"
                                              FontWeight="Bold"
                                              FontSize="11"
                                              Height="20" />
                                <ContentPresenter x:Name="ExpandSite"
                                                  Canvas.Top="25"
                                                  Focusable="false"
                                                  Visibility="Collapsed" />
                            </StackPanel>
                        </Border>
                        <ControlTemplate.Triggers>
                            <Trigger Property="IsExpanded"
                                     Value="true">
                                <Setter Property="Visibility"
                                        TargetName="ExpandSite"
                                        Value="Visible" />
                                <Setter Property="Style"
                                        TargetName="HeaderSite"
                                        Value="{StaticResource ExpanderUpHeaderStyle}" />
                            </Trigger>
                        </ControlTemplate.Triggers>
                    </ControlTemplate>
                </Setter.Value>
            </Setter>
        </Style>

        <Style x:Key="BlackTextBox"
               TargetType="{x:Type TextBox}">
            <Setter Property="Background"
                    Value="{StaticResource Input}" />
            <Setter Property="Foreground"
                    Value="{StaticResource FontColor}" />
            <Setter Property="BorderBrush"
                    Value="{StaticResource Input}" />

            <Setter Property="Template">
                <Setter.Value>
                    <ControlTemplate TargetType="{x:Type TextBox}">
                        <Border x:Name="border"
                                BorderBrush="{TemplateBinding BorderBrush}"
                                BorderThickness="1"
                                CornerRadius="4"
                                Background="{TemplateBinding Background}"
                                Height="30">
                            <ScrollViewer x:Name="PART_ContentHost"
                                          Focusable="false"
                                          VerticalAlignment="Center"
                                          HorizontalScrollBarVisibility="Hidden"
                                          VerticalScrollBarVisibility="Hidden"
                                          Margin="5" />
                        </Border>
                        <ControlTemplate.Triggers>
                            <Trigger Property="IsMouseOver"
                                     Value="true">
                                <Setter Property="Background"
                                        Value="{StaticResource InputHover}" />
                                <Setter Property="Foreground"
                                        Value="White" />
                            </Trigger>
                            <Trigger Property="IsKeyboardFocused"
                                     Value="True">
                                <Setter Property="BorderBrush"
                                        Value="White" />
                            </Trigger>
                        </ControlTemplate.Triggers>
                    </ControlTemplate>
                </Setter.Value>
            </Setter>
        </Style>

        <Style x:Key="ImageButton"
               TargetType="Button">
            <Setter Property="Background">
                <Setter.Value>
                    <ImageBrush ImageSource="Images/ServerInfo/default_server.png" />
                </Setter.Value>
            </Setter>
            <Setter Property="Template">
                <Setter.Value>
                    <ControlTemplate TargetType="{x:Type Button}">
                        <Border BorderThickness="1"
                                CornerRadius="4"
                                BorderBrush="{TemplateBinding BorderBrush}"
                                Background="{TemplateBinding Background}">
                            <ContentPresenter HorizontalAlignment="{TemplateBinding HorizontalContentAlignment}"
                                              Margin="{TemplateBinding Padding}"
                                              VerticalAlignment="{TemplateBinding VerticalContentAlignment}" />
                        </Border>
                        <ControlTemplate.Triggers>
                            <Trigger Property="IsMouseOver"
                                     Value="True">
                                <Setter Property="BorderBrush"
                                        Value="White" />
                            </Trigger>
                        </ControlTemplate.Triggers>
                    </ControlTemplate>
                </Setter.Value>
            </Setter>
        </Style>
        <Style x:Key="DeleteIconButton"
               TargetType="Button">
            <Setter Property="Background">
                <Setter.Value>
                    <ImageBrush ImageSource="Images/ServerInfo/delete_icon.png" />
                </Setter.Value>
            </Setter>
            <Setter Property="BorderBrush"
                    Value="Transparent" />
            <Setter Property="Template">
                <Setter.Value>
                    <ControlTemplate TargetType="{x:Type Button}">
                        <Border BorderThickness="0"
                                CornerRadius="4"
                                BorderBrush="{TemplateBinding BorderBrush}"
                                Background="{TemplateBinding Background}">
                            <ContentPresenter HorizontalAlignment="{TemplateBinding HorizontalContentAlignment}"
                                              Margin="{TemplateBinding Padding}"
                                              VerticalAlignment="{TemplateBinding VerticalContentAlignment}" />
                        </Border>
                        <ControlTemplate.Triggers>
                            <Trigger Property="IsMouseOver"
                                     Value="True">
                                <Setter Property="Background">
                                    <Setter.Value>
                                        <ImageBrush ImageSource="Images/ServerInfo/delete_icon_hover.png" />
                                    </Setter.Value>
                                </Setter>
                            </Trigger>
                        </ControlTemplate.Triggers>
                    </ControlTemplate>
                </Setter.Value>
            </Setter>
        </Style>
        <Style x:Key="SendButton"
               TargetType="Button">
            <Setter Property="Background">
                <Setter.Value>
                    <ImageBrush ImageSource="Images/ServerInfo/send.png" />
                </Setter.Value>
            </Setter>
            <Setter Property="BorderBrush"
                    Value="Transparent" />
            <Setter Property="Template">
                <Setter.Value>
                    <ControlTemplate TargetType="{x:Type Button}">
                        <Border x:Name="border"
                                BorderThickness="1"
                                BorderBrush="{TemplateBinding BorderBrush}"
                                CornerRadius="4"
                                Background="{TemplateBinding Background}">
                            <ContentPresenter HorizontalAlignment="{TemplateBinding HorizontalContentAlignment}"
                                              Margin="{TemplateBinding Padding}"
                                              VerticalAlignment="{TemplateBinding VerticalContentAlignment}" />
                        </Border>
                        <ControlTemplate.Triggers>
                            <Trigger Property="IsMouseOver"
                                     Value="True">
                                <Setter Property="Background"
                                        TargetName="border">
                                    <Setter.Value>
                                        <ImageBrush ImageSource="Images/ServerInfo/send_hover.png" />
                                    </Setter.Value>
                                </Setter>
                            </Trigger>
                            <Trigger Property="IsPressed"
                                     Value="True">
                                <Setter Property="BorderBrush"
                                        Value="White" />
                            </Trigger>
                        </ControlTemplate.Triggers>
                    </ControlTemplate>
                </Setter.Value>
            </Setter>
        </Style>

        <Style x:Key="RoundGrayButton"
               TargetType="{x:Type Button}">
            <Setter Property="Foreground"
                    Value="White" />
            <Setter Property="Background"
                    Value="Transparent" />
            <Setter Property="BorderBrush"
                    Value="{StaticResource Brightness3}" />
            <Setter Property="Template">
                <Setter.Value>
                    <ControlTemplate TargetType="{x:Type Button}">
                        <Border BorderThickness="1"
                                CornerRadius="4"
                                BorderBrush="{TemplateBinding BorderBrush}"
                                Background="{TemplateBinding Background}">
                            <ContentPresenter HorizontalAlignment="{TemplateBinding HorizontalContentAlignment}"
                                              Margin="{TemplateBinding Padding}"
                                              VerticalAlignment="{TemplateBinding VerticalContentAlignment}" />
                        </Border>
                        <ControlTemplate.Triggers>
                            <Trigger Property="IsMouseOver"
                                     Value="True">
                                <Setter Property="Background"
                                        Value="{StaticResource Brightness2}" />
                                <Setter Property="BorderBrush"
                                        Value="White" />
                            </Trigger>
                        </ControlTemplate.Triggers>
                    </ControlTemplate>
                </Setter.Value>
            </Setter>
        </Style>
        <Style x:Key="RoundRedButton"
               TargetType="{x:Type Button}">
            <Setter Property="Background"
                    Value="{StaticResource Red}" />
            <Setter Property="Foreground"
                    Value="White" />
            <Setter Property="BorderBrush"
                    Value="{StaticResource Red}" />
            <Setter Property="Template">
                <Setter.Value>
                    <ControlTemplate TargetType="{x:Type Button}">
                        <Border BorderThickness="1"
                                CornerRadius="4"
                                BorderBrush="{TemplateBinding BorderBrush}"
                                Background="{TemplateBinding Background}">
                            <ContentPresenter HorizontalAlignment="{TemplateBinding HorizontalContentAlignment}"
                                              Margin="{TemplateBinding Padding}"
                                              VerticalAlignment="{TemplateBinding VerticalContentAlignment}" />
                        </Border>
                        <ControlTemplate.Triggers>
                            <Trigger Property="IsMouseOver"
                                     Value="True">
                                <Setter Property="Background"
                                        Value="{StaticResource Red Hover}" />
                                <Setter Property="BorderBrush"
                                        Value="White" />
                            </Trigger>
                        </ControlTemplate.Triggers>
                    </ControlTemplate>
                </Setter.Value>
            </Setter>
        </Style>

        <Style x:Key="ConsoleToggle"
               TargetType="{x:Type ToggleButton}">
            <Setter Property="Background"
                    Value="{StaticResource Input}" />
            <Setter Property="BorderBrush"
                    Value="{StaticResource Input}" />
            <Setter Property="Foreground"
                    Value="{StaticResource FontColor}" />
            <Setter Property="IsChecked"
                    Value="True" />
            <Setter Property="Template">
                <Setter.Value>
                    <ControlTemplate TargetType="{x:Type ToggleButton}">
                        <Border x:Name="border"
                                BorderBrush="{TemplateBinding BorderBrush}"
                                BorderThickness="1"
                                CornerRadius="4"
                                Background="{TemplateBinding Background}"
                                SnapsToDevicePixels="true">
                            <ContentPresenter x:Name="contentPresenter"
                                              Focusable="False" />
                        </Border>
                        <ControlTemplate.Triggers>
                            <Trigger Property="IsMouseOver"
                                     Value="True">
                                <Setter Property="Background"
                                        Value="{StaticResource InputHover}" />
                                <Setter Property="Foreground"
                                        Value="White" />
                            </Trigger>
                            <Trigger Property="IsChecked"
                                     Value="True">
                                <Setter Property="BorderBrush"
                                        Value="White" />
                            </Trigger>
                        </ControlTemplate.Triggers>
                    </ControlTemplate>
                </Setter.Value>
            </Setter>
        </Style>
        <Style x:Key="BlackButton"
               TargetType="Button">
            <Setter Property="Background"
                    Value="{StaticResource Input}" />
            <Setter Property="Foreground"
                    Value="{StaticResource FontColor}" />
            <Setter Property="BorderBrush"
                    Value="{StaticResource Input}" />

            <Setter Property="Template">
                <Setter.Value>
                    <ControlTemplate TargetType="{x:Type Button}">
                        <Border x:Name="border"
                                BorderBrush="{TemplateBinding BorderBrush}"
                                BorderThickness="1"
                                CornerRadius="4"
                                Background="{TemplateBinding Background}"
                                Height="30">
                            <ContentPresenter x:Name="contentPresenter"
                                              Focusable="false" />
                        </Border>
                        <ControlTemplate.Triggers>
                            <Trigger Property="IsMouseOver"
                                     Value="True">
                                <Setter Property="Background"
                                        Value="{StaticResource InputHover}" />
                                <Setter Property="Foreground"
                                        Value="White" />

                            </Trigger>
                            <Trigger Property="IsPressed"
                                     Value="True">
                                <Setter Property="BorderBrush"
                                        Value="White" />
                            </Trigger>
                        </ControlTemplate.Triggers>
                    </ControlTemplate>
                </Setter.Value>
            </Setter>
        </Style>
*/