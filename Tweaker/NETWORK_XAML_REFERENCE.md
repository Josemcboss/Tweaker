# ?? XAML COMPLETO - Network Page

## ?? UBICACIÓN
**Archivo**: `Tweaker\MainWindow.xaml`
**Líneas Aproximadas**: 590-625

---

## ? CÓDIGO XAML VERIFICADO (YA IMPLEMENTADO)

```xaml
<!-- ??????????????????????????????????????????????????????????????????? -->
<!-- NETWORK PAGE - OPTIMIZACIÓN DE RED Y PING -->
<!-- ??????????????????????????????????????????????????????????????????? -->
<ScrollViewer x:Name="NetworkPage" Visibility="Collapsed" VerticalScrollBarVisibility="Auto">
    <StackPanel Margin="40">
        <!-- ???????????????????????????????????????? -->
        <!-- HEADER DE LA PÁGINA -->
        <!-- ???????????????????????????????????????? -->
        <TextBlock FontFamily="Segoe UI Emoji" 
                   FontSize="28" 
                   FontWeight="Bold" 
                   Foreground="White" 
                   Margin="0,0,0,30">
            <Run Text="&#x1F4E1;"/>&#x0020;<Run Text="Red &amp; Ping"/>
        </TextBlock>
        
        <TextBlock Text="Optimizaciones TCP/IP para reducir ping" 
                   FontSize="14" 
                   Foreground="#A0A0A0" 
                   Margin="0,0,0,30"/>

        <!-- ???????????????????????????????????????? -->
        <!-- CARD: OPTIMIZACIÓN TCP/IP COMPLETA -->
        <!-- ???????????????????????????????????????? -->
        <Border Background="#1E1E1E" 
                CornerRadius="8" 
                Padding="25">
            <Grid>
                <!-- Grid con 2 columnas: Info + Botones -->
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="*"/>
                    <ColumnDefinition Width="Auto"/>
                </Grid.ColumnDefinitions>
                
                <!-- ???????????????????????????????????????? -->
                <!-- COLUMNA 0: INFORMACIÓN -->
                <!-- ???????????????????????????????????????? -->
                <StackPanel Grid.Column="0">
                    <!-- Título de la optimización -->
                    <TextBlock Text="Optimización TCP/IP Completa" 
                               Style="{StaticResource SectionTitle}"/>
                    
                    <!-- Descripción técnica -->
                    <TextBlock Style="{StaticResource Description}">
                        <Run Text="TcpAckFrequency = 1, TCPNoDelay = 1, NetworkThrottling OFF"/>
                        <LineBreak/>
                        <Run Text="Reduce ping 5-30ms, mejora hitreg, elimina packet loss"/>
                    </TextBlock>
                </StackPanel>
                
                <!-- ???????????????????????????????????????? -->
                <!-- COLUMNA 1: BOTONES ON/OFF -->
                <!-- ???????????????????????????????????????? -->
                <StackPanel Grid.Column="1" 
                            Orientation="Horizontal" 
                            VerticalAlignment="Center">
                    <!-- Botón ON (Verde Gaming) -->
                    <Button Content="ON" 
                            Style="{StaticResource OnButton}" 
                            Width="70" 
                            Margin="0,0,8,0" 
                            Click="BtnNetworkOptimization_On_Click"/>
                    
                    <!-- Botón OFF (Rojo Gaming) -->
                    <Button Content="OFF" 
                            Style="{StaticResource OffButton}" 
                            Width="70" 
                            Click="BtnNetworkOptimization_Off_Click"/>
                </StackPanel>
            </Grid>
        </Border>
    </StackPanel>
</ScrollViewer>
```

---

## ?? ESTILOS USADOS (Window.Resources)

### SectionTitle
```xaml
<Style x:Key="SectionTitle" TargetType="TextBlock">
    <Setter Property="FontSize" Value="16"/>
    <Setter Property="FontWeight" Value="Bold"/>
    <Setter Property="Foreground" Value="White"/>
    <Setter Property="Margin" Value="0,0,0,10"/>
</Style>
```

### Description
```xaml
<Style x:Key="Description" TargetType="TextBlock">
    <Setter Property="FontSize" Value="11"/>
    <Setter Property="Foreground" Value="#A0A0A0"/>
    <Setter Property="TextWrapping" Value="Wrap"/>
    <Setter Property="Margin" Value="0,0,0,5"/>
</Style>
```

### OnButton (Verde Gaming)
```xaml
<Style x:Key="OnButton" TargetType="Button">
    <Setter Property="Background" Value="#0E7A0D"/>
    <Setter Property="Foreground" Value="White"/>
    <Setter Property="BorderBrush" Value="#107C10"/>
    <Setter Property="BorderThickness" Value="1"/>
    <Setter Property="Padding" Value="15,8"/>
    <Setter Property="FontSize" Value="12"/>
    <Setter Property="FontWeight" Value="SemiBold"/>
    <Setter Property="Cursor" Value="Hand"/>
    <Setter Property="Template">
        <Setter.Value>
            <ControlTemplate TargetType="Button">
                <Border Background="{TemplateBinding Background}"
                        BorderBrush="{TemplateBinding BorderBrush}"
                        BorderThickness="{TemplateBinding BorderThickness}"
                        CornerRadius="4"
                        Padding="{TemplateBinding Padding}">
                    <ContentPresenter HorizontalAlignment="Center" VerticalAlignment="Center"/>
                </Border>
            </ControlTemplate>
        </Setter.Value>
    </Setter>
    <Style.Triggers>
        <Trigger Property="IsMouseOver" Value="True">
            <Setter Property="Background" Value="#11910F"/>
            <Setter Property="BorderBrush" Value="#13A813"/>
        </Trigger>
    </Style.Triggers>
</Style>
```

### OffButton (Rojo Gaming)
```xaml
<Style x:Key="OffButton" TargetType="Button">
    <Setter Property="Background" Value="#A80000"/>
    <Setter Property="Foreground" Value="White"/>
    <Setter Property="BorderBrush" Value="#C50500"/>
    <Setter Property="BorderThickness" Value="1"/>
    <Setter Property="Padding" Value="15,8"/>
    <Setter Property="FontSize" Value="12"/>
    <Setter Property="FontWeight" Value="SemiBold"/>
    <Setter Property="Cursor" Value="Hand"/>
    <Setter Property="Template">
        <Setter.Value>
            <ControlTemplate TargetType="Button">
                <Border Background="{TemplateBinding Background}"
                        BorderBrush="{TemplateBinding BorderBrush}"
                        BorderThickness="{TemplateBinding BorderThickness}"
                        CornerRadius="4"
                        Padding="{TemplateBinding Padding}">
                    <ContentPresenter HorizontalAlignment="Center" VerticalAlignment="Center"/>
                </Border>
            </ControlTemplate>
        </Setter.Value>
    </Setter>
    <Style.Triggers>
        <Trigger Property="IsMouseOver" Value="True">
            <Setter Property="Background" Value="#C50500"/>
            <Setter Property="BorderBrush" Value="#E81123"/>
        </Trigger>
    </Style.Triggers>
</Style>
```

---

## ?? NAVEGACIÓN EN SIDEBAR

### Botón en el Sidebar
```xaml
<Button Style="{StaticResource SidebarButton}"
        Click="NavigateToNetwork"
        x:Name="BtnNavNetwork">
    <StackPanel Orientation="Horizontal">
        <TextBlock Text="&#x1F4E1;" 
                   FontFamily="Segoe UI Emoji" 
                   FontSize="16" 
                   Margin="0,0,10,0"/>
        <TextBlock Text="Red &amp; Ping" 
                   VerticalAlignment="Center"/>
    </StackPanel>
</Button>
```

### SidebarButton Style
```xaml
<Style x:Key="SidebarButton" TargetType="Button">
    <Setter Property="Background" Value="Transparent"/>
    <Setter Property="Foreground" Value="#B9BBBE"/>
    <Setter Property="BorderThickness" Value="0"/>
    <Setter Property="Padding" Value="20,15"/>
    <Setter Property="FontSize" Value="14"/>
    <Setter Property="FontWeight" Value="Medium"/>
    <Setter Property="Cursor" Value="Hand"/>
    <Setter Property="HorizontalContentAlignment" Value="Left"/>
    <Setter Property="Template">
        <Setter.Value>
            <ControlTemplate TargetType="Button">
                <Border Background="{TemplateBinding Background}"
                        Padding="{TemplateBinding Padding}"
                        BorderThickness="0,0,3,0"
                        BorderBrush="Transparent"
                        x:Name="border">
                    <ContentPresenter HorizontalAlignment="{TemplateBinding HorizontalContentAlignment}" 
                                    VerticalAlignment="Center"/>
                </Border>
                <ControlTemplate.Triggers>
                    <Trigger Property="IsMouseOver" Value="True">
                        <Setter Property="Background" Value="#2A2D31"/>
                        <Setter Property="Foreground" Value="White"/>
                    </Trigger>
                    <DataTrigger Binding="{Binding Tag, RelativeSource={RelativeSource Self}}" Value="Active">
                        <Setter TargetName="border" Property="BorderBrush" Value="#5865F2"/>
                        <Setter Property="Background" Value="#35363C"/>
                        <Setter Property="Foreground" Value="White"/>
                    </DataTrigger>
                </ControlTemplate.Triggers>
            </ControlTemplate>
        </Setter.Value>
    </Setter>
</Style>
```

---

## ?? ACCESO RÁPIDO DESDE DASHBOARD

### Botón en Dashboard (Categorías Populares)
```xaml
<Button Background="#2A2D31"
        Foreground="White"
        BorderThickness="0"
        Padding="15,10"
        Margin="0,0,10,10"
        Cursor="Hand"
        Click="NavigateToNetwork">
    <StackPanel Orientation="Horizontal">
        <TextBlock Text="&#x1F4E1;" 
                   FontFamily="Segoe UI Emoji" 
                   Margin="0,0,5,0"/>
        <TextBlock Text="Red &amp; Ping"/>
    </StackPanel>
</Button>
```

---

## ?? CODE-BEHIND (MainWindow.xaml.cs)

### NavigateToNetwork Handler
```csharp
private void NavigateToNetwork(object sender, RoutedEventArgs e)
{
    ShowPage(NetworkPage);
    SetActiveButton((Button)sender);
}
```

### ShowPage Method
```csharp
private void ShowPage(UIElement pageToShow)
{
    // Ocultar todas las páginas
    DashboardPage.Visibility = Visibility.Collapsed;
    InputPage.Visibility = Visibility.Collapsed;
    NetworkPage.Visibility = Visibility.Collapsed;
    SystemPage.Visibility = Visibility.Collapsed;
    CleanupPage.Visibility = Visibility.Collapsed;
    GhostPage.Visibility = Visibility.Collapsed;
    AdvancedPage.Visibility = Visibility.Collapsed;

    // Mostrar la página seleccionada
    pageToShow.Visibility = Visibility.Visible;
}
```

### SetActiveButton Method
```csharp
private void SetActiveButton(Button activeButton)
{
    // Remover estado "Active" de todos los botones
    BtnNavDashboard.Tag = null;
    BtnNavInput.Tag = null;
    BtnNavNetwork.Tag = null;
    BtnNavSystem.Tag = null;
    BtnNavCleanup.Tag = null;
    BtnNavGhost.Tag = null;
    BtnNavAdvanced.Tag = null;

    // Marcar el botón activo
    activeButton.Tag = "Active";
}
```

### Network Optimization Click Handlers
```csharp
private void BtnNetworkOptimization_On_Click(object sender, RoutedEventArgs e)
{
    try
    {
        bool success = NetworkOptimization.OptimizeNetwork();
        
        if (success)
        {
            MessageBox.Show(
                "? OPTIMIZACIÓN DE RED APLICADA\n\n" +
                "???????????????????????????????????????\n" +
                "TCP/IP (Por Interfaz):\n" +
                "???????????????????????????????????????\n" +
                "• TcpAckFrequency: 1\n" +
                "  ? ACK inmediato, reduce 10-40ms\n\n" +
                "• TCPNoDelay: 1\n" +
                "  ? Deshabilita Nagle's Algorithm\n\n" +
                "• TcpDelAckTicks: 0\n" +
                "  ? Sin delay artificial\n\n" +
                "???????????????????????????????????????\n" +
                "SISTEMA:\n" +
                "???????????????????????????????????????\n" +
                "• SystemResponsiveness: 0 (TODO el CPU para apps)\n" +
                "• NetworkThrottlingIndex: FFFFFFFF (sin límite)\n\n" +
                "Beneficios:\n" +
                "? Ping reducido en 5-30ms\n" +
                "? Mejor hitreg en shooters\n" +
                "? Menos packet loss\n" +
                "? Elimina lag artificial\n\n" +
                "?? REINICIA Windows.",
                "Optimización de Red - REINICIAR",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
    catch (Exception ex)
    {
        MessageBox.Show($"? Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}

private void BtnNetworkOptimization_Off_Click(object sender, RoutedEventArgs e)
{
    try
    {
        bool success = NetworkOptimization.RestoreNetwork();
        
        if (success)
        {
            MessageBox.Show(
                "? CONFIGURACIÓN DE RED RESTAURADA\n\n" +
                "TCP/IP:\n" +
                "• TcpAckFrequency, TCPNoDelay, TcpDelAckTicks: ELIMINADOS\n\n" +
                "SISTEMA:\n" +
                "• NetworkThrottlingIndex: 10 (default)\n" +
                "• SystemResponsiveness: 20 (default)\n\n" +
                "?? REINICIA Windows.",
                "Configuración Restaurada - REINICIAR",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
    catch (Exception ex)
    {
        MessageBox.Show($"? Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
```

---

## ?? PALETA DE COLORES

### Colores del Tema (Discord/Hone.gg Style)

```
Background Principal:     #0F0F0F (Casi negro)
Background Sidebar:       #1E1E1E (Gris oscuro)
Background Cards:         #1E1E1E (Gris oscuro)
Background Hover:         #2A2D31 (Gris medio)
Background Active:        #35363C (Gris medio activo)

Texto Principal:          White (#FFFFFF)
Texto Secundario:         #B9BBBE (Gris claro)
Texto Descripción:        #A0A0A0 (Gris medio)
Texto Footer:             #7F8084 (Gris oscuro)

Acento Principal:         #5865F2 (Azul Discord)
Botón ON (Verde):         #0E7A0D (Verde gaming)
Botón ON Hover:           #11910F (Verde claro)
Botón OFF (Rojo):         #A80000 (Rojo gaming)
Botón OFF Hover:          #C50500 (Rojo claro)

Warning:                  #FFC107 (Amarillo)
Error:                    #E81123 (Rojo error)
GHOST Theme:              #9B59B6 (Púrpura)
```

---

## ?? DIMENSIONES Y ESPACIADO

```
Margin de Página:         40px
Padding de Cards:         25px
Border Radius:            8px

Botón Width:              70px (ON/OFF)
Botón Padding:            15px, 8px
Botón Margin:             8px (entre botones)

Título Página:            FontSize 28px, Bold
Subtítulo:                FontSize 14px
Título Sección:           FontSize 16px, Bold
Descripción:              FontSize 11px

Icono Emoji:              FontSize 16px (sidebar)
                          FontSize 28px (página header)
```

---

## ?? EMOJIS USADOS

```
Red/Network:              &#x1F4E1; (??)
Rayo/Speed:               &#x26A1; (?)
Check/Success:            ?
Warning:                  ??
Error:                    ?
Gaming:                   &#x1F3AE; (??)
```

---

## ? VERIFICACIÓN FINAL

- [x] ScrollViewer "NetworkPage" definido
- [x] Visibility="Collapsed" por defecto
- [x] Header con emoji y título
- [x] Card con Border y CornerRadius
- [x] Grid de 2 columnas (Info + Botones)
- [x] Título con Style SectionTitle
- [x] Descripción con Style Description
- [x] Botones ON/OFF con estilos correctos
- [x] Click handlers conectados
- [x] Botón en Sidebar
- [x] Botón en Dashboard
- [x] Navegación funcional

---

**TODO EL XAML ESTÁ IMPLEMENTADO Y FUNCIONAL** ?
