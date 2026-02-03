# ========================================
# SCRIPT PARA COMPLETAR TODAS LAS PÁGINAS
# ========================================

Write-Host "Completando páginas del Dashboard..." -ForegroundColor Cyan

# Leer archivo actual
$xaml = Get-Content "MainWindow.xaml" -Encoding UTF8 -Raw

# Definir el contenido de Input Page
$inputContent = @'
                    <TextBlock Text="Optimizaciones de Input Lag y FPS" FontSize="14" Foreground="#A0A0A0" Margin="0,0,0,30"/>

                    <!-- Keyboard -->
                    <Border Background="#1E1E1E" CornerRadius="8" Padding="25" Margin="0,0,0,15">
                        <Grid>
                            <Grid.ColumnDefinitions><ColumnDefinition Width="*"/><ColumnDefinition Width="Auto"/></Grid.ColumnDefinitions>
                            <StackPanel Grid.Column="0">
                                <TextBlock Text="Optimizar Teclado" Style="{StaticResource SectionTitle}"/>
                                <TextBlock Style="{StaticResource Description}" Text="KeyboardDelay = 0, Input lag -50ms"/>
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
                            <Grid.ColumnDefinitions><ColumnDefinition Width="*"/><ColumnDefinition Width="Auto"/></Grid.ColumnDefinitions>
                            <StackPanel Grid.Column="0">
                                <TextBlock Text="Efectos Visuales OFF" Style="{StaticResource SectionTitle}"/>
                                <TextBlock Style="{StaticResource Description}" Text="FPS +3-8%, GPU +5-10%"/>
                            </StackPanel>
                            <StackPanel Grid.Column="1" Orientation="Horizontal" VerticalAlignment="Center">
                                <Button Content="ON" Style="{StaticResource OnButton}" Width="70" Margin="0,0,8,0" Click="BtnVisuals_On_Click"/>
                                <Button Content="OFF" Style="{StaticResource OffButton}" Width="70" Click="BtnVisuals_Off_Click"/>
                            </StackPanel>
                        </Grid>
                    </Border>

                    <!-- Memory -->
                    <Border Background="#1E1E1E" CornerRadius="8" Padding="25">
                        <Grid>
                            <Grid.ColumnDefinitions><ColumnDefinition Width="*"/><ColumnDefinition Width="Auto"/></Grid.ColumnDefinitions>
                            <StackPanel Grid.Column="0">
                                <TextBlock Text="Optimizar RAM" Style="{StaticResource SectionTitle}"/>
                                <TextBlock Style="{StaticResource Description}" Text="DisablePagingExecutive = 1, Requiere 16GB+"/>
                            </StackPanel>
                            <StackPanel Grid.Column="1" Orientation="Horizontal" VerticalAlignment="Center">
                                <Button Content="ON" Style="{StaticResource OnButton}" Width="70" Margin="0,0,8,0" Click="BtnMemory_On_Click"/>
                                <Button Content="OFF" Style="{StaticResource OffButton}" Width="70" Click="BtnMemory_Off_Click"/>
                            </StackPanel>
                        </Grid>
                    </Border>
'@

# Buscar y reemplazar InputPage vacía
$inputPagePattern = '(?s)(<ScrollViewer x:Name="InputPage"[^>]*>.*?<StackPanel Margin="40">.*?FontSize="28".*?Margin="0,0,0,\d+"/>)(.*?)(</StackPanel>\s*</ScrollViewer>)'
if ($xaml -match $inputPagePattern) {
    $xaml = $xaml -replace $inputPagePattern, "`$1`n$inputContent`n                `$3"
    Write-Host "? Input Page completada" -ForegroundColor Green
} else {
    Write-Host "? No se pudo actualizar Input Page" -ForegroundColor Yellow
}

# Guardar
$xaml | Set-Content "MainWindow.xaml" -Encoding UTF8 -NoNewline

Write-Host "`nScript completado. Compila la aplicación para verificar." -ForegroundColor Cyan
