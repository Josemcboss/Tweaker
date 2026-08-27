import sys

with open("Tweaker/MainWindow.xaml", "r", encoding="utf-8") as f:
    lines = f.readlines()

def get_block(start_line, end_line):
    # start_line and end_line are 1-based indices
    return "".join(lines[start_line-1:end_line])

# Extract sections
header = get_block(504, 518)
stats_cards = get_block(519, 562)
telemetry_title = lines[563-1]
graphs_grid = get_block(564, 605)
smart_scan = get_block(606, 636)
auto_maintenance = get_block(637, 674)
game_mode = get_block(675, 693)
perf_title = get_block(694, 695)
perf_buttons = get_block(696, 700)
perf_grid = get_block(701, 794)
perf_info = get_block(795, 798)
guide = get_block(799, 806)
active_tweaks = get_block(807, 814)
quick_actions = get_block(815, 868)

# Now, let's modify specific parts to clean them up for 2-column layout:

# 1. Update stats cards margin
stats_cards = stats_cards.replace('Margin="0,0,10,0"', 'Margin="0,0,8,0"')
stats_cards = stats_cards.replace('Margin="5,0"', 'Margin="4,0"')
stats_cards = stats_cards.replace('Margin="10,0,0,0"', 'Margin="8,0,0,0"')

# 2. Update Smart Scan margin (remove Margin="0,20,0,0", replace with Margin="0,0,0,20")
smart_scan = smart_scan.replace('Margin="0,20,0,0"', 'Margin="0,0,0,20"')

# 3. Update Auto Maintenance margin
auto_maintenance = auto_maintenance.replace('Margin="0,0,0,20"', 'Margin="0,0,0,20"')

# 4. Update Game Mode margin
game_mode = game_mode.replace('Margin="0,0,0,20"', 'Margin="0,0,0,20"')

# 5. Update Quick Actions margin
quick_actions = quick_actions.replace('Margin="0,20,0,0"', 'Margin="0,0,0,20"')

# 6. Rebuild Telemetry section as vertical stack with Height="85" graphs
telemetry_vertical = """            <TextBlock Text="Monitoreo de Hardware" FontSize="14" Foreground="#A0A0A0" Margin="0,0,0,12" FontWeight="SemiBold" />
            <Border Style="{StaticResource NeonGlowCard}" Margin="0,0,0,12" Padding="15">
              <StackPanel>
                <Grid Margin="0,0,0,8">
                  <TextBlock Text="CPU" FontWeight="Bold" Foreground="#5865F2" HorizontalAlignment="Left" />
                  <StackPanel Orientation="Horizontal" HorizontalAlignment="Right">
                    <TextBlock Name="TxtCpuTemp" Text="0°C" Foreground="#A0A0A0" Margin="0,0,10,0" VerticalAlignment="Center" />
                    <TextBlock Name="TxtCpuLoad" Text="0%" Foreground="#FFFFFFFF" FontWeight="Bold" VerticalAlignment="Center" />
                  </StackPanel>
                </Grid>
                <controls:HardwareGraph x:Name="GraphCpu" Height="85" LineColor="DodgerBlue" MaxValue="100" ValueUnit="%" />
              </StackPanel>
            </Border>
            <Border Style="{StaticResource NeonGlowCard}" Margin="0,0,0,12" Padding="15">
              <StackPanel>
                <Grid Margin="0,0,0,8">
                  <TextBlock Text="GPU" FontWeight="Bold" Foreground="#2ECC71" HorizontalAlignment="Left" />
                  <StackPanel Orientation="Horizontal" HorizontalAlignment="Right">
                    <TextBlock Name="TxtGpuTemp" Text="0°C" Foreground="#A0A0A0" Margin="0,0,10,0" VerticalAlignment="Center" />
                    <TextBlock Name="TxtGpuLoad" Text="0%" Foreground="#FFFFFFFF" FontWeight="Bold" VerticalAlignment="Center" />
                  </StackPanel>
                </Grid>
                <controls:HardwareGraph x:Name="GraphGpu" Height="85" LineColor="SpringGreen" MaxValue="100" ValueUnit="%" />
              </StackPanel>
            </Border>
            <Border Style="{StaticResource NeonGlowCard}" Margin="0,0,0,20" Padding="15">
              <StackPanel>
                <Grid Margin="0,0,0,8">
                  <TextBlock Text="RAM" FontWeight="Bold" Foreground="#9B59B6" HorizontalAlignment="Left" />
                  <StackPanel Orientation="Horizontal" HorizontalAlignment="Right">
                    <TextBlock Name="TxtRamLoad" Text="0%" Foreground="#FFFFFFFF" FontWeight="Bold" VerticalAlignment="Center" />
                  </StackPanel>
                </Grid>
                <controls:HardwareGraph x:Name="GraphRam" Height="85" LineColor="MediumPurple" MaxValue="100" ValueUnit="%" />
              </StackPanel>
            </Border>
"""

# 7. Modify Performance Comparison buttons to WrapPanel for responsive wrap
perf_buttons_wrapped = """            <WrapPanel Margin="0,0,0,15">
              <Button Content="📸 BASELINE" Style="{StaticResource OnButton}" Padding="12,8" Margin="0,0,8,8" Background="#5865F2" BorderBrush="#7289DA" Command="{Binding TakeBaselineSnapshotCommand}" />
              <Button Content="📸 OPTIMIZADO" Style="{StaticResource OnButton}" Padding="12,8" Margin="0,0,8,8" Command="{Binding TakeOptimizedSnapshotCommand}" />
              <Button Content="🗑️ LIMPIAR" Style="{StaticResource OffButton}" Padding="12,8" Margin="0,0,0,8" Command="{Binding ClearMetricsHistoryCommand}" />
            </WrapPanel>
"""

# 8. Modify performance grid margins to fit column
perf_grid = perf_grid.replace('Margin="0,0,8,0"', 'Margin="0,0,4,0"')
perf_grid = perf_grid.replace('Margin="4,0"', 'Margin="4,0"')
perf_grid = perf_grid.replace('Margin="8,0,0,0"', 'Margin="4,0,0,0"')

# 9. Modify performance info margin
perf_info = perf_info.replace('Margin="0,0,0,15"', 'Margin="0,0,0,20"')

# 10. Update Active Tweaks Section top margin
active_tweaks = active_tweaks.replace('Margin="0,20,0,0"', 'Margin="0,0,0,20"')

# 11. Update Guide margin
guide = guide.replace('Margin="0,20,0,0"', 'Margin="0,0,0,20"')

# Create the full 2-column Grid content
two_column_grid = f"""          <Grid Margin="30,25,30,25">
            <Grid.RowDefinitions>
              <RowDefinition Height="Auto" />
              <RowDefinition Height="*" />
            </Grid.RowDefinitions>
            <Grid.ColumnDefinitions>
              <ColumnDefinition Width="1.35*" />
              <ColumnDefinition Width="1*" />
            </Grid.ColumnDefinitions>

            <!-- Row 0: Header -->
            <StackPanel Grid.Row="0" Grid.ColumnSpan="2" Margin="0,0,0,20">
{header}            </StackPanel>

            <!-- Row 1, Column 0: Left Column (Main controls & tweaks) -->
            <StackPanel Grid.Row="1" Grid.Column="0" Margin="0,0,15,0">
{stats_cards}
{smart_scan}
{auto_maintenance}
{game_mode}
{quick_actions}            </StackPanel>

            <!-- Row 1, Column 1: Right Column (Telemetry & Performance stats) -->
            <StackPanel Grid.Row="1" Grid.Column="1" Margin="15,0,0,0">
{telemetry_vertical}
{perf_title}
{perf_buttons_wrapped}
{perf_grid}
{perf_info}
{active_tweaks}
{guide}            </StackPanel>
          </Grid>
"""

before_dashboard = "".join(lines[:502]) # Up to line 502 (inclusive), which is </UIElement.RenderTransform>
after_dashboard = "".join(lines[869:]) # From line 870 (inclusive), which is </ScrollViewer>

new_xaml = before_dashboard + "\n" + two_column_grid + after_dashboard

with open("Tweaker/MainWindow.xaml", "w", encoding="utf-8") as f:
    f.write(new_xaml)

print("MainWindow.xaml successfully restructured!")
