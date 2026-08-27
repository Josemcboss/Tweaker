import re

xaml_path = 'Tweaker/MainWindow.xaml'

with open(xaml_path, 'r', encoding='utf-8') as f:
    xaml = f.read()

# Locate DashboardPage ScrollViewer start and end
start_tag = '<ScrollViewer x:Name="DashboardPage"'
end_tag = '</ScrollViewer>'

start_idx = xaml.find(start_tag)
if start_idx == -1:
    print("DashboardPage ScrollViewer not found!")
    exit(1)

# Find the matching closing tag
depth = 0
pos = start_idx
end_idx = -1
while pos < len(xaml):
    if xaml.startswith('<ScrollViewer', pos):
        if pos + 13 < len(xaml) and xaml[pos+13] in (' ', '>', '\n', '\r'):
            depth += 1
        pos += 13
    elif xaml.startswith('</ScrollViewer>', pos):
        depth -= 1
        pos += 15
        if depth == 0:
            end_idx = pos
            break
    else:
        pos += 1

if end_idx == -1:
    print("Closing </ScrollViewer> not found!")
    exit(1)

dashboard_xml = xaml[start_idx:end_idx]

# Let's extract the components using exact regexes
# 1. Stats Cards
stats_match = re.search(r'(<!-- Stats Cards \(DINÁMICAS\) -->.*?<!-- Real-time Hardware Monitoring -->)', dashboard_xml, re.DOTALL)
if not stats_match:
    stats_match = re.search(r'(<!-- Stats Cards.*?<!-- Real-time Hardware Monitoring -->)', dashboard_xml, re.DOTALL)
stats_cards = stats_match.group(1).replace('<!-- Real-time Hardware Monitoring -->', '').strip()

# 2. Smart Scan
scan_match = re.search(r'(<!-- Smart Scan Section \(NUEVO\) -->.*?<!-- ═══════ ROW 2: AUTO-MAINTENANCE ═══════ -->)', dashboard_xml, re.DOTALL)
if not scan_match:
    scan_match = re.search(r'(<!-- Smart Scan Section.*?<!-- ═══════ ROW 2: AUTO-MAINTENANCE ═══════ -->)', dashboard_xml, re.DOTALL)
smart_scan = scan_match.group(1).replace('<!-- ═══════ ROW 2: AUTO-MAINTENANCE ═══════ -->', '').strip()

# 3. Auto Maintenance
maintenance_match = re.search(r'(<!-- ═══════ ROW 2: AUTO-MAINTENANCE ═══════ -->.*?<!-- ═══════ ROW 3: GAME MODE STATUS ═══════ -->)', dashboard_xml, re.DOTALL)
auto_maintenance = maintenance_match.group(1).replace('<!-- ═══════ ROW 3: GAME MODE STATUS ═══════ -->', '').strip()

# 4. Game Mode
game_mode_match = re.search(r'(<!-- ═══════ ROW 3: GAME MODE STATUS ═══════ -->.*?<!-- ═══════ ROW 4: BEFORE/AFTER COMPARISON ═══════ -->)', dashboard_xml, re.DOTALL)
game_mode = game_mode_match.group(1).replace('<!-- ═══════ ROW 4: BEFORE/AFTER COMPARISON ═══════ -->', '').strip()

# 5. Performance Comparison
comparison_match = re.search(r'(<TextBlock Text="📈 COMPARATIVA DE RENDIMIENTO".*?<!-- Info Section -->)', dashboard_xml, re.DOTALL)
comparison = comparison_match.group(1).replace('<!-- Info Section -->', '').strip()

# 6. Info Section (Guía Rápida)
info_match = re.search(r'(<!-- Info Section -->.*?<!-- Active Tweaks Section)', dashboard_xml, re.DOTALL)
info_section = info_match.group(1).replace('<!-- Active Tweaks Section', '').strip()

# 7. Active Tweaks Section
active_match = re.search(r'(<!-- Active Tweaks Section \(NUEVO - DINÁMICO\) -->.*?<!-- Quick Actions -->)', dashboard_xml, re.DOTALL)
active_tweaks = active_match.group(1).replace('<!-- Quick Actions -->', '').strip()

# 8. Quick Actions & Categories
quick_match = re.search(r'(<!-- Quick Actions -->.*)', dashboard_xml, re.DOTALL)
quick_actions_raw = quick_match.group(1).strip()

border_depth = 0
qa_pos = 0
qa_end = -1
while qa_pos < len(quick_actions_raw):
    if quick_actions_raw.startswith('<Border', qa_pos):
        if not quick_actions_raw.startswith('<BorderBrush', qa_pos):
            border_depth += 1
        qa_pos += 7
    elif quick_actions_raw.startswith('</Border>', qa_pos):
        border_depth -= 1
        qa_pos += 9
        if border_depth == 0:
            qa_end = qa_pos
            break
    else:
        qa_pos += 1

quick_actions = quick_actions_raw[:qa_end].strip()

# Define the new vertical stacked graphs
graphs = """
                        <!-- Real-time Hardware Monitoring -->
                        <TextBlock Text="Monitoreo de Hardware en Tiempo Real" 
                               FontSize="14" 
                               Foreground="#A0A0A0"
                               Margin="0,0,0,12"/>
                        
                        <StackPanel Margin="0,0,0,20">
                            <!-- CPU Graph -->
                            <Border Style="{StaticResource NeonGlowCard}" Margin="0,0,0,10" Padding="12">
                                <StackPanel>
                                    <Grid Margin="0,0,0,8">
                                        <TextBlock Text="CPU" FontWeight="Bold" Foreground="#5865F2" HorizontalAlignment="Left"/>
                                        <StackPanel Orientation="Horizontal" HorizontalAlignment="Right">
                                            <TextBlock x:Name="TxtCpuTemp" Text="0°C" Foreground="#A0A0A0" Margin="0,0,10,0" VerticalAlignment="Center"/>
                                            <TextBlock x:Name="TxtCpuLoad" Text="0%" Foreground="White" FontWeight="Bold" VerticalAlignment="Center"/>
                                        </StackPanel>
                                    </Grid>
                                    <controls:HardwareGraph x:Name="GraphCpu" Height="80" LineColor="DodgerBlue" MaxValue="100" ValueUnit="%"/>
                                </StackPanel>
                            </Border>

                            <!-- GPU Graph -->
                            <Border Style="{StaticResource NeonGlowCard}" Margin="0,0,0,10" Padding="12">
                                <StackPanel>
                                    <Grid Margin="0,0,0,8">
                                        <TextBlock Text="GPU" FontWeight="Bold" Foreground="#2ECC71" HorizontalAlignment="Left"/>
                                        <StackPanel Orientation="Horizontal" HorizontalAlignment="Right">
                                            <TextBlock x:Name="TxtGpuTemp" Text="0°C" Foreground="#A0A0A0" Margin="0,0,10,0" VerticalAlignment="Center"/>
                                            <TextBlock x:Name="TxtGpuLoad" Text="0%" Foreground="White" FontWeight="Bold" VerticalAlignment="Center"/>
                                        </StackPanel>
                                    </Grid>
                                    <controls:HardwareGraph x:Name="GraphGpu" Height="80" LineColor="SpringGreen" MaxValue="100" ValueUnit="%"/>
                                </StackPanel>
                            </Border>

                            <!-- RAM Graph -->
                            <Border Style="{StaticResource NeonGlowCard}" Margin="0,0,0,10" Padding="12">
                                <StackPanel>
                                    <Grid Margin="0,0,0,8">
                                        <TextBlock Text="RAM" FontWeight="Bold" Foreground="#9B59B6" HorizontalAlignment="Left"/>
                                        <StackPanel Orientation="Horizontal" HorizontalAlignment="Right">
                                            <TextBlock x:Name="TxtRamLoad" Text="0%" Foreground="White" FontWeight="Bold" VerticalAlignment="Center"/>
                                        </StackPanel>
                                    </Grid>
                                    <controls:HardwareGraph x:Name="GraphRam" Height="80" LineColor="MediumPurple" MaxValue="100" ValueUnit="%"/>
                                </StackPanel>
                            </Border>
                        </StackPanel>
"""

# Reconstruct the new ScrollViewer content - fixing ColumnDefinition tag name
new_dashboard_content = f"""<ScrollViewer x:Name="DashboardPage" 
                          Visibility="Visible" 
                          VerticalScrollBarVisibility="Auto">
                    <ScrollViewer.RenderTransform>
                        <TranslateTransform/>
                    </ScrollViewer.RenderTransform>

                    <Grid Margin="30,25,30,25">
                        <Grid.RowDefinitions>
                            <RowDefinition Height="Auto"/> <!-- Row 0: Header -->
                            <RowDefinition Height="*"/>    <!-- Row 1: Columns -->
                        </Grid.RowDefinitions>
                        <Grid.ColumnDefinitions>
                            <ColumnDefinition Width="1.35*"/> <!-- Col 0: Main controls & Scan -->
                            <ColumnDefinition Width="1*"/>    <!-- Col 1: Telemetry & Monitoring -->
                        </Grid.ColumnDefinitions>

                        <!-- ROW 0: HEADER (Spans both columns) -->
                        <StackPanel Grid.Row="0" Grid.ColumnSpan="2" Margin="0,0,0,20">
                            <StackPanel Orientation="Horizontal" Margin="0,0,0,8">
                                <resources:GhostOptimizerLogo Width="50" Height="50" Margin="0,0,15,0" VerticalAlignment="Center"/>
                                <StackPanel VerticalAlignment="Center">
                                    <TextBlock FontFamily="Impact, Arial Black" FontSize="28" FontWeight="Bold" Foreground="White">
                                        <Run Text="GHOST OPTIMIZER"/>
                                        <TextBlock.Effect>
                                            <DropShadowEffect Color="#A970FF" BlurRadius="8" ShadowDepth="0" Opacity="0.6"/>
                                        </TextBlock.Effect>
                                    </TextBlock>
                                    <TextBlock FontFamily="Segoe UI" FontSize="14" Foreground="#B9BBBE" Margin="0,3,0,0">
                                        <Run Text="Gaming Performance Dashboard"/>
                                    </TextBlock>
                                </StackPanel>
                            </StackPanel>
                            <TextBlock Text="Optimizador Gaming Profesional" FontSize="13" Foreground="#A0A0A0"/>
                        </StackPanel>

                        <!-- ROW 1, COL 0: MAIN TWEAKS & ACTIONS (LEFT COLUMN) -->
                        <StackPanel Grid.Row="1" Grid.Column="0" Margin="0,0,15,0">
                            {stats_cards}
                            
                            {smart_scan}
                            
                            {auto_maintenance}
                            
                            {game_mode}
                            
                            {quick_actions}
                        </StackPanel>

                        <!-- ROW 1, COL 1: REAL-TIME TELEMETRY & STATS (RIGHT COLUMN) -->
                        <StackPanel Grid.Row="1" Grid.Column="1" Margin="15,0,0,0">
                            {graphs}
                            
                            {comparison}
                            
                            {active_tweaks}
                            
                            {info_section}
                        </StackPanel>
                    </Grid>
                </ScrollViewer>"""

# Replace in xaml
new_xaml = xaml[:start_idx] + new_dashboard_content + xaml[end_idx:]

# Just in case, if the previous run already replaced it but failed, let's replace Grid.ColumnDefinition with ColumnDefinition directly on the whole file
new_xaml = new_xaml.replace('<Grid.ColumnDefinition ', '<ColumnDefinition ')
new_xaml = new_xaml.replace('</Grid.ColumnDefinition>', '</ColumnDefinition>')

with open(xaml_path, 'w', encoding='utf-8') as f:
    f.write(new_xaml)

print("Dashboard restructured and fixed successfully!")
