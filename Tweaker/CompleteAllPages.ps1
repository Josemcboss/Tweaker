# ===============================================
# SCRIPT FINAL: COMPLETAR TODAS LAS PÁGINAS
# ===============================================

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "  COMPLETANDO DASHBOARD - TODAS LAS PÁGINAS" -ForegroundColor Cyan
Write-Host "========================================`n" -ForegroundColor Cyan

$xaml = Get-Content "MainWindow.xaml" -Encoding UTF8 -Raw

# ===== NETWORK PAGE =====
$networkContent = @'
                    <TextBlock Text="Optimizaciones TCP/IP para reducir ping" FontSize="14" Foreground="#A0A0A0" Margin="0,0,0,30"/>

                    <Border Background="#1E1E1E" CornerRadius="8" Padding="25">
                        <Grid>
                            <Grid.ColumnDefinitions><ColumnDefinition Width="*"/><ColumnDefinition Width="Auto"/></Grid.ColumnDefinitions>
                            <StackPanel Grid.Column="0">
                                <TextBlock Text="Optimización TCP/IP Completa" Style="{StaticResource SectionTitle}"/>
                                <TextBlock Style="{StaticResource Description}">
                                    <Run Text="TcpAckFrequency = 1, TCPNoDelay = 1, NetworkThrottling OFF"/>
                                    <LineBreak/>
                                    <Run Text="Reduce ping 5-30ms, mejora hitreg, elimina packet loss"/>
                                </TextBlock>
                            </StackPanel>
                            <StackPanel Grid.Column="1" Orientation="Horizontal" VerticalAlignment="Center">
                                <Button Content="ON" Style="{StaticResource OnButton}" Width="70" Margin="0,0,8,0" Click="BtnNetworkOptimization_On_Click"/>
                                <Button Content="OFF" Style="{StaticResource OffButton}" Width="70" Click="BtnNetworkOptimization_Off_Click"/>
                            </StackPanel>
                        </Grid>
                    </Border>
'@

$networkPattern = '(?s)(<ScrollViewer x:Name="NetworkPage"[^>]*>.*?<StackPanel Margin="40">.*?Red &amp; Ping.*?Margin="0,0,0,\d+"/>)(.*?)(</StackPanel>\s*</ScrollViewer>)'
if ($xaml -match $networkPattern) {
    $xaml = $xaml -replace $networkPattern, "`$1`n$networkContent`n                `$3"
    Write-Host "? Network Page completada" -ForegroundColor Green
}

# ===== SYSTEM PAGE =====
$systemContent = @'
                    <TextBlock Text="GPU, CPU y Configuración del Sistema" FontSize="14" Foreground="#A0A0A0" Margin="0,0,0,30"/>

                    <!-- System Profile -->
                    <Border Background="#1E1E1E" CornerRadius="8" Padding="25" Margin="0,0,0,15">
                        <Grid>
                            <Grid.ColumnDefinitions><ColumnDefinition Width="*"/><ColumnDefinition Width="Auto"/></Grid.ColumnDefinitions>
                            <StackPanel Grid.Column="0">
                                <TextBlock Text="System Profile Games Priority" Style="{StaticResource SectionTitle}"/>
                                <TextBlock Style="{StaticResource Description}" Text="GPU Priority: 8, CPU Priority: 6, Scheduling: High"/>
                            </StackPanel>
                            <StackPanel Grid.Column="1" Orientation="Horizontal" VerticalAlignment="Center">
                                <Button Content="ON" Style="{StaticResource OnButton}" Width="70" Margin="0,0,8,0" Click="BtnSystemProfile_On_Click"/>
                                <Button Content="OFF" Style="{StaticResource OffButton}" Width="70" Click="BtnSystemProfile_Off_Click"/>
                            </StackPanel>
                        </Grid>
                    </Border>

                    <!-- GameDVR -->
                    <Border Background="#1E1E1E" CornerRadius="8" Padding="25" Margin="0,0,0,15">
                        <Grid>
                            <Grid.ColumnDefinitions><ColumnDefinition Width="*"/><ColumnDefinition Width="Auto"/></Grid.ColumnDefinitions>
                            <StackPanel Grid.Column="0">
                                <TextBlock Text="Deshabilitar GameDVR (Xbox Game Bar)" Style="{StaticResource SectionTitle}"/>
                                <TextBlock Style="{StaticResource Description}" Text="Elimina overlay, reduce input lag 5-15ms"/>
                            </StackPanel>
                            <StackPanel Grid.Column="1" Orientation="Horizontal" VerticalAlignment="Center">
                                <Button Content="ON" Style="{StaticResource OnButton}" Width="70" Margin="0,0,8,0" Click="BtnGameDVR_On_Click"/>
                                <Button Content="OFF" Style="{StaticResource OffButton}" Width="70" Click="BtnGameDVR_Off_Click"/>
                            </StackPanel>
                        </Grid>
                    </Border>

                    <!-- GPU Scheduling -->
                    <Border Background="#1E1E1E" CornerRadius="8" Padding="25" Margin="0,0,0,15">
                        <Grid>
                            <Grid.ColumnDefinitions><ColumnDefinition Width="*"/><ColumnDefinition Width="Auto"/></Grid.ColumnDefinitions>
                            <StackPanel Grid.Column="0">
                                <TextBlock Text="Hardware GPU Scheduling" Style="{StaticResource SectionTitle}"/>
                                <TextBlock Style="{StaticResource Description}" Text="Puede mejorar o empeorar latencia (probar ambos)"/>
                            </StackPanel>
                            <StackPanel Grid.Column="1" Orientation="Horizontal" VerticalAlignment="Center">
                                <Button Content="ON" Style="{StaticResource OnButton}" Width="70" Margin="0,0,8,0" Click="BtnGpuScheduling_On_Click"/>
                                <Button Content="OFF" Style="{StaticResource OffButton}" Width="70" Click="BtnGpuScheduling_Off_Click"/>
                            </StackPanel>
                        </Grid>
                    </Border>

                    <!-- System Responsiveness -->
                    <Border Background="#1E1E1E" CornerRadius="8" Padding="25" Margin="0,0,0,15">
                        <Grid>
                            <Grid.ColumnDefinitions><ColumnDefinition Width="*"/><ColumnDefinition Width="Auto"/></Grid.ColumnDefinitions>
                            <StackPanel Grid.Column="0">
                                <TextBlock Text="System Responsiveness" Style="{StaticResource SectionTitle}"/>
                                <TextBlock Style="{StaticResource Description}" Text="SystemResponsiveness: 0, NetworkThrottling OFF"/>
                            </StackPanel>
                            <StackPanel Grid.Column="1" Orientation="Horizontal" VerticalAlignment="Center">
                                <Button Content="ON" Style="{StaticResource OnButton}" Width="70" Margin="0,0,8,0" Click="BtnSystemResponsiveness_On_Click"/>
                                <Button Content="OFF" Style="{StaticResource OffButton}" Width="70" Click="BtnSystemResponsiveness_Off_Click"/>
                            </StackPanel>
                        </Grid>
                    </Border>

                    <!-- High Performance -->
                    <Border Background="#1E1E1E" CornerRadius="8" Padding="25" Margin="0,0,0,15">
                        <Grid>
                            <Grid.ColumnDefinitions><ColumnDefinition Width="*"/><ColumnDefinition Width="Auto"/></Grid.ColumnDefinitions>
                            <StackPanel Grid.Column="0">
                                <TextBlock Text="Plan de Energía: Alto Rendimiento" Style="{StaticResource SectionTitle}"/>
                                <TextBlock Style="{StaticResource Description}" Text="CPU siempre a máxima frecuencia"/>
                            </StackPanel>
                            <StackPanel Grid.Column="1" Orientation="Horizontal" VerticalAlignment="Center">
                                <Button Content="ON" Style="{StaticResource OnButton}" Width="70" Margin="0,0,8,0" Click="BtnHighPerformance_On_Click"/>
                                <Button Content="OFF" Style="{StaticResource OffButton}" Width="70" Click="BtnHighPerformance_Off_Click"/>
                            </StackPanel>
                        </Grid>
                    </Border>

                    <!-- Power Throttling -->
                    <Border Background="#1E1E1E" CornerRadius="8" Padding="25" Margin="0,0,0,15">
                        <Grid>
                            <Grid.ColumnDefinitions><ColumnDefinition Width="*"/><ColumnDefinition Width="Auto"/></Grid.ColumnDefinitions>
                            <StackPanel Grid.Column="0">
                                <TextBlock Text="Deshabilitar Power Throttling" Style="{StaticResource SectionTitle}"/>
                                <TextBlock Style="{StaticResource Description}" Text="Elimina throttling de apps en background"/>
                            </StackPanel>
                            <StackPanel Grid.Column="1" Orientation="Horizontal" VerticalAlignment="Center">
                                <Button Content="ON" Style="{StaticResource OnButton}" Width="70" Margin="0,0,8,0" Click="BtnPowerThrottling_On_Click"/>
                                <Button Content="OFF" Style="{StaticResource OffButton}" Width="70" Click="BtnPowerThrottling_Off_Click"/>
                            </StackPanel>
                        </Grid>
                    </Border>

                    <!-- Core Parking -->
                    <Border Background="#1E1E1E" CornerRadius="8" Padding="25">
                        <Grid>
                            <Grid.ColumnDefinitions><ColumnDefinition Width="*"/><ColumnDefinition Width="Auto"/></Grid.ColumnDefinitions>
                            <StackPanel Grid.Column="0">
                                <TextBlock Text="Deshabilitar Core Parking" Style="{StaticResource SectionTitle}"/>
                                <TextBlock Style="{StaticResource Description}" Text="Mantiene todos los cores activos (crítico en Ryzen)"/>
                            </StackPanel>
                            <StackPanel Grid.Column="1" Orientation="Horizontal" VerticalAlignment="Center">
                                <Button Content="ON" Style="{StaticResource OnButton}" Width="70" Margin="0,0,8,0" Click="BtnCoreParking_On_Click"/>
                                <Button Content="OFF" Style="{StaticResource OffButton}" Width="70" Click="BtnCoreParking_Off_Click"/>
                            </StackPanel>
                        </Grid>
                    </Border>
'@

$systemPattern = '(?s)(<ScrollViewer x:Name="SystemPage"[^>]*>.*?<StackPanel Margin="40">.*?Sistema &amp; GPU.*?Margin="0,0,0,\d+"/>)(.*?)(</StackPanel>\s*</ScrollViewer>)'
if ($xaml -match $systemPattern) {
    $xaml = $xaml -replace $systemPattern, "`$1`n$systemContent`n                `$3"
    Write-Host "? System Page completada" -ForegroundColor Green
}

# ===== CLEANUP PAGE =====
$cleanupContent = @'
                    <TextBlock Text="Deshabilitar servicios y bloatware de Windows" FontSize="14" Foreground="#A0A0A0" Margin="0,0,0,30"/>

                    <!-- Hibernation -->
                    <Border Background="#1E1E1E" CornerRadius="8" Padding="25" Margin="0,0,0,15">
                        <Grid>
                            <Grid.ColumnDefinitions><ColumnDefinition Width="*"/><ColumnDefinition Width="Auto"/></Grid.ColumnDefinitions>
                            <StackPanel Grid.Column="0">
                                <TextBlock Text="Deshabilitar Hibernación" Style="{StaticResource SectionTitle}"/>
                                <TextBlock Style="{StaticResource Description}" Text="Elimina hiberfil.sys (8-32GB)"/>
                            </StackPanel>
                            <StackPanel Grid.Column="1" Orientation="Horizontal" VerticalAlignment="Center">
                                <Button Content="ON" Style="{StaticResource OnButton}" Width="70" Margin="0,0,8,0" Click="BtnHibernation_On_Click"/>
                                <Button Content="OFF" Style="{StaticResource OffButton}" Width="70" Click="BtnHibernation_Off_Click"/>
                            </StackPanel>
                        </Grid>
                    </Border>

                    <!-- Windows Search -->
                    <Border Background="#1E1E1E" CornerRadius="8" Padding="25" Margin="0,0,0,15">
                        <Grid>
                            <Grid.ColumnDefinitions><ColumnDefinition Width="*"/><ColumnDefinition Width="Auto"/></Grid.ColumnDefinitions>
                            <StackPanel Grid.Column="0">
                                <TextBlock Text="Deshabilitar Windows Search" Style="{StaticResource SectionTitle}"/>
                                <TextBlock Style="{StaticResource Description}" Text="Detiene indexación, libera 200-500MB RAM"/>
                            </StackPanel>
                            <StackPanel Grid.Column="1" Orientation="Horizontal" VerticalAlignment="Center">
                                <Button Content="ON" Style="{StaticResource OnButton}" Width="70" Margin="0,0,8,0" Click="BtnWindowsSearch_On_Click"/>
                                <Button Content="OFF" Style="{StaticResource OffButton}" Width="70" Click="BtnWindowsSearch_Off_Click"/>
                            </StackPanel>
                        </Grid>
                    </Border>

                    <!-- SysMain -->
                    <Border Background="#1E1E1E" CornerRadius="8" Padding="25" Margin="0,0,0,15">
                        <Grid>
                            <Grid.ColumnDefinitions><ColumnDefinition Width="*"/><ColumnDefinition Width="Auto"/></Grid.ColumnDefinitions>
                            <StackPanel Grid.Column="0">
                                <TextBlock Text="Deshabilitar SysMain (SuperFetch)" Style="{StaticResource SectionTitle}"/>
                                <TextBlock Style="{StaticResource Description}" Text="Libera 1-3GB RAM, reduce uso de disco"/>
                            </StackPanel>
                            <StackPanel Grid.Column="1" Orientation="Horizontal" VerticalAlignment="Center">
                                <Button Content="ON" Style="{StaticResource OnButton}" Width="70" Margin="0,0,8,0" Click="BtnSysMainService_On_Click"/>
                                <Button Content="OFF" Style="{StaticResource OffButton}" Width="70" Click="BtnSysMainService_Off_Click"/>
                            </StackPanel>
                        </Grid>
                    </Border>

                    <!-- Telemetry -->
                    <Border Background="#1E1E1E" CornerRadius="8" Padding="25">
                        <Grid>
                            <Grid.ColumnDefinitions><ColumnDefinition Width="*"/><ColumnDefinition Width="Auto"/></Grid.ColumnDefinitions>
                            <StackPanel Grid.Column="0">
                                <TextBlock Text="Deshabilitar Telemetry (DiagTrack)" Style="{StaticResource SectionTitle}"/>
                                <TextBlock Style="{StaticResource Description}" Text="Bloquea envío de datos a Microsoft"/>
                            </StackPanel>
                            <StackPanel Grid.Column="1" Orientation="Horizontal" VerticalAlignment="Center">
                                <Button Content="ON" Style="{StaticResource OnButton}" Width="70" Margin="0,0,8,0" Click="BtnDiagTrack_On_Click"/>
                                <Button Content="OFF" Style="{StaticResource OffButton}" Width="70" Click="BtnDiagTrack_Off_Click"/>
                            </StackPanel>
                        </Grid>
                    </Border>
'@

$cleanupPattern = '(?s)(<ScrollViewer x:Name="CleanupPage"[^>]*>.*?<StackPanel Margin="40">.*?Limpieza.*?Margin="0,0,0,\d+"/>)(.*?)(</StackPanel>\s*</ScrollViewer>)'
if ($xaml -match $cleanupPattern) {
    $xaml = $xaml -replace $cleanupPattern, "`$1`n$cleanupContent`n                `$3"
    Write-Host "? Cleanup Page completada" -ForegroundColor Green
}

# ===== FR33THY PAGE =====
$fr33thyContent = @'
                    <TextBlock Text="Tweaks legendarios de FR33THY (YouTube 500K+)" FontSize="14" Foreground="#FF6B35" Margin="0,0,0,30"/>

                    <!-- MPO -->
                    <Border Background="#1E1E1E" CornerRadius="8" Padding="25" Margin="0,0,0,15">
                        <Grid>
                            <Grid.ColumnDefinitions><ColumnDefinition Width="*"/><ColumnDefinition Width="Auto"/></Grid.ColumnDefinitions>
                            <StackPanel Grid.Column="0">
                                <TextBlock Text="Deshabilitar MPO (Multiplane Overlay)" Style="{StaticResource SectionTitle}"/>
                                <TextBlock Style="{StaticResource Description}" Text="Elimina pantallazos negros y stuttering severo"/>
                            </StackPanel>
                            <StackPanel Grid.Column="1" Orientation="Horizontal" VerticalAlignment="Center">
                                <Button Content="ON" Style="{StaticResource OnButton}" Width="70" Margin="0,0,8,0" Click="BtnMPO_On_Click"/>
                                <Button Content="OFF" Style="{StaticResource OffButton}" Width="70" Click="BtnMPO_Off_Click"/>
                            </StackPanel>
                        </Grid>
                    </Border>

                    <!-- Ultimate Performance -->
                    <Border Background="#1E1E1E" CornerRadius="8" Padding="25" Margin="0,0,0,15">
                        <Grid>
                            <Grid.ColumnDefinitions><ColumnDefinition Width="*"/><ColumnDefinition Width="Auto"/></Grid.ColumnDefinitions>
                            <StackPanel Grid.Column="0">
                                <TextBlock Text="Plan Ultimate Performance (Oculto)" Style="{StaticResource SectionTitle}"/>
                                <TextBlock Style="{StaticResource Description}" Text="Latencia CPU -93%, C-States OFF, 0.1% low FPS +20%"/>
                            </StackPanel>
                            <StackPanel Grid.Column="1" Orientation="Horizontal" VerticalAlignment="Center">
                                <Button Content="ON" Style="{StaticResource OnButton}" Width="70" Margin="0,0,8,0" Click="BtnUltimatePower_On_Click"/>
                                <Button Content="OFF" Style="{StaticResource OffButton}" Width="70" Click="BtnUltimatePower_Off_Click"/>
                            </StackPanel>
                        </Grid>
                    </Border>

                    <!-- Game Bar -->
                    <Border Background="#1E1E1E" CornerRadius="8" Padding="25" Margin="0,0,0,15">
                        <Grid>
                            <Grid.ColumnDefinitions><ColumnDefinition Width="*"/><ColumnDefinition Width="Auto"/></Grid.ColumnDefinitions>
                            <StackPanel Grid.Column="0">
                                <TextBlock Text="Deshabilitar Xbox Game Bar" Style="{StaticResource SectionTitle}"/>
                                <TextBlock Style="{StaticResource Description}" Text="Input lag -50%, CPU libre +8%"/>
                            </StackPanel>
                            <StackPanel Grid.Column="1" Orientation="Horizontal" VerticalAlignment="Center">
                                <Button Content="ON" Style="{StaticResource OnButton}" Width="70" Margin="0,0,8,0" Click="BtnGameBar_On_Click"/>
                                <Button Content="OFF" Style="{StaticResource OffButton}" Width="70" Click="BtnGameBar_Off_Click"/>
                            </StackPanel>
                        </Grid>
                    </Border>

                    <!-- Core Isolation -->
                    <Border Background="#1E1E1E" CornerRadius="8" Padding="25" Margin="0,0,0,15">
                        <Grid>
                            <Grid.ColumnDefinitions><ColumnDefinition Width="*"/><ColumnDefinition Width="Auto"/></Grid.ColumnDefinitions>
                            <StackPanel Grid.Column="0">
                                <TextBlock Text="Deshabilitar Core Isolation (VBS)" Style="{StaticResource SectionTitle}"/>
                                <TextBlock Style="{StaticResource Description}">
                                    <Run Text="Memory Integrity causa -10 a -30% FPS" FontWeight="Bold" Foreground="#E81123"/>
                                    <LineBreak/>
                                    <Run Text="FPS +10-30%, Input lag -3ms, REQUIERE REINICIO" FontWeight="SemiBold"/>
                                </TextBlock>
                            </StackPanel>
                            <StackPanel Grid.Column="1" Orientation="Horizontal" VerticalAlignment="Center">
                                <Button Content="ON" Style="{StaticResource OnButton}" Width="70" Margin="0,0,8,0" Click="BtnCoreIsolation_On_Click"/>
                                <Button Content="OFF" Style="{StaticResource OffButton}" Width="70" Click="BtnCoreIsolation_Off_Click"/>
                            </StackPanel>
                        </Grid>
                    </Border>

                    <!-- HPET -->
                    <Border Background="#1E1E1E" CornerRadius="8" Padding="25" Margin="0,0,0,15">
                        <Grid>
                            <Grid.ColumnDefinitions><ColumnDefinition Width="*"/><ColumnDefinition Width="Auto"/></Grid.ColumnDefinitions>
                            <StackPanel Grid.Column="0">
                                <TextBlock Text="Deshabilitar HPET" Style="{StaticResource SectionTitle}"/>
                                <TextBlock Style="{StaticResource Description}" Text="Reduce micro-stuttering (Ryzen), REQUIERE REINICIO"/>
                            </StackPanel>
                            <StackPanel Grid.Column="1" Orientation="Horizontal" VerticalAlignment="Center">
                                <Button Content="ON" Style="{StaticResource OnButton}" Width="70" Margin="0,0,8,0" Click="BtnHPET_On_Click"/>
                                <Button Content="OFF" Style="{StaticResource OffButton}" Width="70" Click="BtnHPET_Off_Click"/>
                            </StackPanel>
                        </Grid>
                    </Border>

                    <!-- Hyper-V -->
                    <Border Background="#1E1E1E" CornerRadius="8" Padding="25">
                        <Grid>
                            <Grid.ColumnDefinitions><ColumnDefinition Width="*"/><ColumnDefinition Width="Auto"/></Grid.ColumnDefinitions>
                            <StackPanel Grid.Column="0">
                                <TextBlock Text="Deshabilitar Hyper-V" Style="{StaticResource SectionTitle}"/>
                                <TextBlock Style="{StaticResource Description}" Text="Reduce latencia GPU 2-5ms, REQUIERE REINICIO"/>
                            </StackPanel>
                            <StackPanel Grid.Column="1" Orientation="Horizontal" VerticalAlignment="Center">
                                <Button Content="ON" Style="{StaticResource OnButton}" Width="70" Margin="0,0,8,0" Click="BtnHyperV_On_Click"/>
                                <Button Content="OFF" Style="{StaticResource OffButton}" Width="70" Click="BtnHyperV_Off_Click"/>
                            </StackPanel>
                        </Grid>
                    </Border>
'@

$fr33thyPattern = '(?s)(<ScrollViewer x:Name="Fr33thyPage"[^>]*>.*?<StackPanel Margin="40">.*?FR33THY PACK.*?Margin="0,0,0,\d+"/>)(.*?)(</StackPanel>\s*</ScrollViewer>)'
if ($xaml -match $fr33thyPattern) {
    $xaml = $xaml -replace $fr33thyPattern, "`$1`n$fr33thyContent`n                `$3"
    Write-Host "? FR33THY Page completada" -ForegroundColor Green
}

# Guardar archivo final
$xaml | Set-Content "MainWindow.xaml" -Encoding UTF8 -NoNewline

Write-Host "`n========================================" -ForegroundColor Green
Write-Host "  ? TODAS LAS PÁGINAS COMPLETADAS" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host "`nCompilando para verificar..." -ForegroundColor Cyan
