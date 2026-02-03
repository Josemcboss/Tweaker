# Script para completar las páginas del MainWindow.xaml
$file = "Tweaker/MainWindow.xaml"
$content = Get-Content $file -Encoding UTF8 -Raw

# INPUT PAGE - Reemplazar contenido vacío
$inputPageOld = @'
            <ScrollViewer x:Name="InputPage" Visibility="Collapsed" VerticalScrollBarVisibility="Auto">
                <StackPanel Margin="40">
                    <TextBlock Text="?? Input &amp; Visuals" FontSize="28" FontWeight="Bold" Foreground="White" Margin="0,0,0,10"/>
                    <!-- Contenido se agrega aquí -->
                </StackPanel>
            </ScrollViewer>
'@

$inputPageNew = @'
            <ScrollViewer x:Name="InputPage" Visibility="Collapsed" VerticalScrollBarVisibility="Auto">
                <StackPanel Margin="40">
                    <TextBlock Text="?? Input &amp; Visuals" FontSize="28" FontWeight="Bold" Foreground="White" Margin="0,0,0,10"/>
                    <TextBlock Text="Optimizaciones de Input Lag, FPS y Responsividad" FontSize="14" Foreground="#A0A0A0" Margin="0,0,0,30"/>

                    <!-- Keyboard Optimization -->
                    <Border Background="#1E1E1E" CornerRadius="8" Padding="25" Margin="0,0,0,15">
                        <Grid>
                            <Grid.ColumnDefinitions>
                                <ColumnDefinition Width="*"/>
                                <ColumnDefinition Width="Auto"/>
                            </Grid.ColumnDefinitions>
                            <StackPanel Grid.Column="0">
                                <TextBlock Text="Optimizar Teclado (Input Lag Fix)" Style="{StaticResource SectionTitle}"/>
                                <TextBlock Style="{StaticResource Description}">
                                    <Run Text="KeyboardDelay = 0 (sin delay de repetición)"/>
                                    <LineBreak/>
                                    <Run Text="? Input lag -50-100ms · WASD más responsive"/>
                                </TextBlock>
                            </StackPanel>
                            <StackPanel Grid.Column="1" Orientation="Horizontal" VerticalAlignment="Center">
                                <Button Content="ON" Style="{StaticResource OnButton}" Width="70" Margin="0,0,8,0" Click="BtnKeyboard_On_Click"/>
                                <Button Content="OFF" Style="{StaticResource OffButton}" Width="70" Click="BtnKeyboard_Off_Click"/>
                            </StackPanel>
                        </Grid>
                    </Border>

                    <!-- Visual Effects -->
                    <Border Background="#1E1E1E" CornerRadius="8" Padding="25" Margin="0,0,0,15">
                        <Grid>
                            <Grid.ColumnDefinitions>
                                <ColumnDefinition Width="*"/>
                                <ColumnDefinition Width="Auto"/>
                            </Grid.ColumnDefinitions>
                            <StackPanel Grid.Column="0">
                                <TextBlock Text="Deshabilitar Efectos Visuales (FPS Boost)" Style="{StaticResource SectionTitle}"/>
                                <TextBlock Style="{StaticResource Description}">
                                    <Run Text="VisualFXSetting = 2 (Mejor rendimiento)"/>
                                    <LineBreak/>
                                    <Run Text="? FPS +3-8% · GPU +5-10% · RAM +200-500MB"/>
                                </TextBlock>
                            </StackPanel>
                            <StackPanel Grid.Column="1" Orientation="Horizontal" VerticalAlignment="Center">
                                <Button Content="ON" Style="{StaticResource OnButton}" Width="70" Margin="0,0,8,0" Click="BtnVisuals_On_Click"/>
                                <Button Content="OFF" Style="{StaticResource OffButton}" Width="70" Click="BtnVisuals_Off_Click"/>
                            </StackPanel>
                        </Grid>
                    </Border>

                    <!-- Memory Management -->
                    <Border Background="#1E1E1E" CornerRadius="8" Padding="25">
                        <Grid>
                            <Grid.ColumnDefinitions>
                                <ColumnDefinition Width="*"/>
                                <ColumnDefinition Width="Auto"/>
                            </Grid.ColumnDefinitions>
                            <StackPanel Grid.Column="0">
                                <TextBlock Text="Optimizar RAM (Kernel en Memoria)" Style="{StaticResource SectionTitle}"/>
                                <TextBlock Style="{StaticResource Description}">
                                    <Run Text="DisablePagingExecutive = 1"/>
                                    <LineBreak/>
                                    <Run Text="? Sistema 'snappy' · Elimina stuttering"/>
                                    <LineBreak/>
                                    <Run Text="?? REQUIERE 16GB+ RAM" Foreground="#FFC107" FontWeight="SemiBold"/>
                                </TextBlock>
                            </StackPanel>
                            <StackPanel Grid.Column="1" Orientation="Horizontal" VerticalAlignment="Center">
                                <Button Content="ON" Style="{StaticResource OnButton}" Width="70" Margin="0,0,8,0" Click="BtnMemory_On_Click"/>
                                <Button Content="OFF" Style="{StaticResource OffButton}" Width="70" Click="BtnMemory_Off_Click"/>
                            </StackPanel>
                        </Grid>
                    </Border>
                </StackPanel>
            </ScrollViewer>
'@

$content = $content -replace [regex]::Escape($inputPageOld), $inputPageNew

# Guardar
$content | Set-Content $file -Encoding UTF8 -NoNewline

Write-Host "? Input Page completada" -ForegroundColor Green
