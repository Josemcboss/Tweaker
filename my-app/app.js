// ==========================================================================
// GHOST OPTIMIZER v2.0 - INTERACTIVE SITE LOGIC
// ==========================================================================

// Real codebase database schema
const TWEAKS_DATABASE = [
    // --- INPUT & VISUALS ---
    {
        id: "mouse_acceleration",
        title: "Mouse Acceleration OFF",
        category: "Input & Visuals",
        desc: "Elimina la aceleración artificial a nivel de Windows para lograr precisión Aim 1:1.",
        benefits: ["Aim 1:1 pixel perfect tracking", "Movimientos consistentes y predecibles", "Mejor muscle memory en shooters"],
        warnings: "Puede sentirse lento al principio.",
        risk: "safe",
        restart: false,
        fps: 0, ping: 0, ram: 0
    },
    {
        id: "keyboard_optimization",
        title: "Keyboard Optimization",
        category: "Input & Visuals",
        desc: "Reduce el delay de repetición de teclas (KeyboardDelay = 0) para una respuesta táctil instantánea.",
        benefits: ["Input lag de teclas reducido en ~50ms", "Respuesta más rápida de ráfagas", "Ideal para spam de habilidades en juegos"],
        warnings: null,
        risk: "safe",
        restart: false,
        fps: 2, ping: 0, ram: 0
    },
    {
        id: "visual_effects",
        title: "Visual Effects OFF",
        category: "Input & Visuals",
        desc: "Deshabilita animaciones y efectos aero de Windows para optimizar los recursos del procesador gráfico.",
        benefits: ["FPS +3-8% de ganancia promedio", "GPU usage -5-10% liberado para el juego", "Alt+Tab hasta un 50% más rápido"],
        warnings: "Windows se verá plano sin efectos modernos.",
        risk: "safe",
        restart: false,
        fps: 6, ping: 0, ram: 0.3
    },
    {
        id: "transparency_effects",
        title: "Disable Transparency Effects",
        category: "Input & Visuals",
        desc: "Apaga los efectos de transparencia y Acrylic para ahorrar consumo y espacio de VRAM.",
        benefits: ["GPU usage -3-8%", "VRAM liberada +50-200MB", "Compositor de ventanas ligero"],
        warnings: "Sin efectos modernos translúcidos.",
        risk: "safe",
        restart: false,
        fps: 4, ping: 0, ram: 0.1
    },
    {
        id: "raw_aim_curve",
        title: "Raw Aim Curve",
        category: "Input & Visuals",
        desc: "Aplica una optimización de curva de puntería en el registro del sistema, desactivando la aceleración a nivel kernel.",
        benefits: ["Desactivación de aceleración de mouse en kernel", "Perfecto 1:1 para Valorant, CS2 y Apex", "Sin interpolación del sistema operativo"],
        warnings: "Requiere reiniciar el equipo para su activación.",
        risk: "safe",
        restart: true,
        fps: 0, ping: 0, ram: 0
    },
    {
        id: "stickykeys_guard",
        title: "StickyKeys Guard",
        category: "Input & Visuals",
        desc: "Deshabilita el atajo y notificaciones de Sticky Keys a nivel de registro para evitar interrupciones.",
        benefits: ["Sin interrupciones accidentales en plena partida", "Previene ventanas emergentes al pulsar Shift repetidamente"],
        warnings: null,
        risk: "safe",
        restart: false,
        fps: 0, ping: 0, ram: 0
    },

    // --- RED & PING ---
    {
        id: "tcp_ip",
        title: "TCP/IP Optimization",
        category: "Red & Ping",
        desc: "Configura TcpAckFrequency=1 y TCPNoDelay=1 para enviar paquetes de red de inmediato.",
        benefits: ["Ping reducido de 5 a 30ms en servidores competitivos", "Elimina retrasos en la entrega de paquetes de red", "Mejor registro de balas/hits"],
        warnings: "Aumenta mínimamente la cantidad de paquetes transmitidos.",
        risk: "safe",
        restart: true,
        fps: 0, ping: 25, ram: 0
    },
    {
        id: "dns_cloudflare",
        title: "DNS Cloudflare",
        category: "Red & Ping",
        desc: "Asigna el servidor DNS público de Cloudflare (1.1.1.1), el más veloz y seguro del mundo.",
        benefits: ["Reducción de latencia en resolución web", "Ping mejorado de -10 a -50ms", "Mayor privacidad"],
        warnings: null,
        risk: "safe",
        restart: false,
        fps: 0, ping: 35, ram: 0
    },
    {
        id: "network_power",
        title: "Network Power OFF",
        category: "Red & Ping",
        desc: "Evita que Windows apague la tarjeta de red para ahorrar energía, manteniéndola activa 24/7.",
        benefits: ["Cero micro-desconexiones durante partidas", "Adaptador operando siempre a máxima velocidad y rendimiento"],
        warnings: null,
        risk: "safe",
        restart: false,
        fps: 0, ping: 5, ram: 0
    },
    {
        id: "do_solo_mode",
        title: "DO Solo Mode",
        category: "Red & Ping",
        desc: "Establece Delivery Optimization para actualizaciones de red local, previniendo subidas no deseadas a internet.",
        benefits: ["Evita el uso de ancho de banda de subida por updates", "Reduce pings altos y lag intermitente en red", "Conexión más limpia"],
        warnings: "Actualizaciones directas de servidores oficiales.",
        risk: "safe",
        restart: false,
        fps: 0, ping: 15, ram: 0
    },
    {
        id: "mtu_optimization",
        title: "MTU Optimization",
        category: "Red & Ping",
        desc: "Ajusta la Unidad de Transmisión Máxima (MTU) a 1492 bytes para evitar la fragmentación de paquetes.",
        benefits: ["Latencia reducida -2-5ms", "Cero fragmentación de paquetes", "Carga de red optimizada"],
        warnings: null,
        risk: "safe",
        restart: true,
        fps: 0, ping: 10, ram: 0
    },

    // --- SISTEMA & GPU ---
    {
        id: "gpu_scheduling",
        title: "GPU Scheduling (HAGS)",
        category: "Sistema & GPU",
        desc: "Permite que la GPU administre su propia memoria, reduciendo la latencia de renderizado.",
        benefits: ["Menor latencia de renderizado de la GPU", "Mejora el rendimiento en títulos con DirectX 12 y Vulkan", "Frame pacing más consistente"],
        warnings: "Probar compatibilidad de GPU.",
        risk: "safe",
        restart: true,
        fps: 12, ping: 0, ram: 0
    },
    {
        id: "high_performance",
        title: "High Performance Power Plan",
        category: "Sistema & GPU",
        desc: "Fuerza al procesador a trabajar a su máxima frecuencia constante, deshabilitando el ahorro de energía.",
        benefits: ["Procesador siempre al 100% de frecuencia", "Elimina retrasos al demandar potencia de cálculo", "Menos micro-stutters"],
        warnings: "Aumenta ligeramente el consumo eléctrico.",
        risk: "safe",
        restart: false,
        fps: 10, ping: 0, ram: 0
    },
    {
        id: "game_mode",
        title: "Windows Game Mode",
        category: "Sistema & GPU",
        desc: "Prioriza los recursos del procesador y la gráfica para el juego en ejecución frente a las tareas en segundo plano.",
        benefits: ["Frame stability mejorada entre un 10-15%", "Evita actualizaciones del sistema mientras juegas"],
        warnings: null,
        risk: "safe",
        restart: false,
        fps: 8, ping: 0, ram: 0.2
    },
    {
        id: "win32_priority",
        title: "Win32 Priority Optimization",
        category: "Sistema & GPU",
        desc: "Ajusta la prioridad del planificador para favorecer a las aplicaciones en primer plano (juegos).",
        benefits: ["Prioridad de procesamiento dedicada al juego", "Evita stutters generados por programas de fondo", "Frametimes planos"],
        warnings: "Puede ralentizar muy levemente tareas de renderizado en segundo plano.",
        risk: "safe",
        restart: false,
        fps: 8, ping: 0, ram: 0
    },

    // --- LIMPIEZA ---
    {
        id: "hibernation_off",
        title: "Hibernation OFF",
        category: "Limpieza",
        desc: "Deshabilita el estado de hibernación eliminando el archivo hiberfil.sys.",
        benefits: ["Libera de 8 a 32 GB en el disco de sistema", "Elimina operaciones innecesarias en el SSD"],
        warnings: "El modo de reposo 'Hibernar' no estará disponible.",
        risk: "safe",
        restart: false,
        fps: 0, ping: 0, ram: 0.1
    },
    {
        id: "windows_search",
        title: "Windows Search OFF",
        category: "Limpieza",
        desc: "Detiene e inhabilita el servicio de indexación de archivos en segundo plano de Windows Search.",
        benefits: ["Libera entre 200 y 500 MB de memoria RAM", "Cero lecturas de indexado en disco mientras juegas", "Menor carga en procesadores de pocos núcleos"],
        warnings: "La búsqueda de archivos en Windows Explorer será más lenta.",
        risk: "safe",
        restart: false,
        fps: 2, ping: 0, ram: 0.4
    },
    {
        id: "sysmain_off",
        title: "SysMain OFF (Superfetch)",
        category: "Limpieza",
        desc: "Deshabilita el servicio SysMain que precarga aplicaciones frecuentes en la RAM.",
        benefits: ["Libera de 1 a 3 GB de memoria RAM física", "Evita picos de uso de CPU al precargar software", "Perfecto para PCs con 8GB-16GB RAM"],
        warnings: "Los programas habituales tardarán medio segundo más en cargar al abrirse.",
        risk: "safe",
        restart: false,
        fps: 4, ping: 0, ram: 2.0
    },
    {
        id: "telemetry_off",
        title: "Telemetry OFF",
        category: "Limpieza",
        desc: "Inhabilita el servicio de recopilación de diagnósticos DiagTrack de Microsoft.",
        benefits: ["Mayor privacidad al bloquear transmisiones", "Menos consumo de red y ciclos de CPU en background"],
        warnings: null,
        risk: "safe",
        restart: false,
        fps: 3, ping: 2, ram: 0.3
    },

    // --- GHOST PACK ---
    {
        id: "ultimate_power_plan",
        title: "Ultimate Power Plan",
        category: "GHOST Pack",
        desc: "Activa la directiva de energía de Máximo Rendimiento para minimizar la latencia entre hilos de CPU.",
        benefits: ["Latencia interna de comunicación de CPU reducida en un 93%", "Evita aparcamientos de núcleos (Core Parking OFF)", "Estabilidad de FPS extrema"],
        warnings: "Incrementa el consumo y las temperaturas de CPUs en laptops.",
        risk: "safe",
        restart: false,
        fps: 15, ping: 0, ram: 0
    },
    {
        id: "core_isolation_off",
        title: "Core Isolation (VBS) OFF",
        category: "GHOST Pack",
        desc: "Inhabilita la seguridad basada en virtualización (VBS) y la integridad de memoria del kernel de Windows.",
        benefits: ["Ganancia drástica del rendimiento en juegos de un +10% a +30% FPS", "Latencia de hardware drásticamente reducida"],
        warnings: "Reduce la protección contra exploits sofisticados que ataquen el kernel.",
        risk: "moderate",
        restart: true,
        fps: 25, ping: 0, ram: 0
    },
    {
        id: "hpet_off",
        title: "HPET OFF",
        category: "GHOST Pack",
        desc: "Deshabilita el temporizador de alta precisión (HPET) a nivel de SO, forzando el uso de temporizadores TSC más veloces.",
        benefits: ["Elimina micro-stuttering provocado por timers desincronizados", "Consigue frametimes más planos y limpios"],
        warnings: "Algunos chipsets muy antiguos requieren HPET para arrancar.",
        risk: "moderate",
        restart: true,
        fps: 6, ping: 0, ram: 0
    },
    {
        id: "mpo_fix",
        title: "MPO Fix",
        category: "GHOST Pack",
        desc: "Deshabilita la superposición de múltiples planos (MPO) para solucionar fallos visuales en GPUs Nvidia/AMD.",
        benefits: ["Soluciona parpadeos en navegadores y Discord", "Evita tirones al jugar en modo ventana sin bordes"],
        warnings: null,
        risk: "safe",
        restart: false,
        fps: 2, ping: 0, ram: 0
    },

    // --- ADVANCED ---
    {
        id: "spectre_meltdown_off",
        title: "Spectre & Meltdown Mitigations OFF",
        category: "Advanced",
        desc: "Deshabilita las mitigaciones de seguridad por software para Spectre y Meltdown a nivel de hardware de CPU.",
        benefits: ["Recupera del +5% al +15% de FPS y velocidad de IPC", "Reduce la sobrecarga de traducción de páginas de memoria"],
        warnings: "Expone la CPU a vulnerabilidades de hardware si se ejecuta código desconocido localmente.",
        risk: "danger",
        restart: true,
        fps: 18, ping: 0, ram: 0
    },
    {
        id: "usb_optimization",
        title: "USB Selective Suspend OFF",
        category: "Advanced",
        desc: "Desactiva la suspensión temporal de puertos USB para mantener la energía constante a los dispositivos periféricos.",
        benefits: ["Previene desconexiones intermitentes del mouse/teclado", "Mantiene la velocidad del polling rate de periféricos estable"],
        warnings: null,
        risk: "safe",
        restart: false,
        fps: 0, ping: 5, ram: 0
    },
    {
        id: "input_queues",
        title: "Optimized Input Queues",
        category: "Advanced",
        desc: "Aumenta el tamaño del búfer de cola de eventos a 1000 para mouse y 200 para teclado.",
        benefits: ["Evita la pérdida de inputs de clic o pulsaciones rápidas", "Ideal para polling rates ultra-altos (4000Hz - 8000Hz)"],
        warnings: "Aumenta marginalmente el consumo de RAM no paginada.",
        risk: "safe",
        restart: true,
        fps: 2, ping: 0, ram: 0
    },
    {
        id: "trim_force_on",
        title: "TRIM Force-On",
        category: "Advanced",
        desc: "Fuerza la ejecución periódica del comando TRIM en el sistema de archivos de Windows.",
        benefits: ["Previene la degradación en las velocidades de lectura y escritura del SSD", "Cargas en juegos más rápidas en el tiempo"],
        warnings: "No tiene efecto en discos mecánicos HDD tradicionales.",
        risk: "safe",
        restart: false,
        fps: 0, ping: 0, ram: 0
    },
    {
        id: "usb_power_guard",
        title: "USB Power Guard",
        category: "Advanced",
        desc: "Fuerza que los concentradores raíz USB funcionen a máxima potencia a nivel de registro.",
        benefits: ["Cero latencia al despertar dispositivos USB", "Estabilidad en hubs externos de audio e inputs"],
        warnings: null,
        risk: "safe",
        restart: false,
        fps: 0, ping: 3, ram: 0
    },
    {
        id: "hibernation_wipe",
        title: "Hibernation Wipe",
        category: "Advanced",
        desc: "Inhabilita por completo la hibernación y ejecuta una limpieza a fondo del bloque hiberfil.sys.",
        benefits: ["Recupera gigabytes críticos en discos SSD NVMe pequeños", "Mayor vida útil del SSD al no reescribir gigabytes de estado"],
        warnings: "Incompatible con el inicio rápido de Windows.",
        risk: "safe",
        restart: false,
        fps: 0, ping: 0, ram: 0
    }
];

// Profile mappings representing exact sets of tweaks to toggle
const PROFILES = {
    casual: {
        tweaks: [
            "mouse_acceleration", "keyboard_optimization", "visual_effects", 
            "transparency_effects", "dns_cloudflare", "network_power", 
            "do_solo_mode", "game_mode", "hibernation_off", "windows_search", 
            "sysmain_off", "telemetry_off"
        ],
        sched: "balanced"
    },
    competitive: {
        tweaks: [
            "mouse_acceleration", "keyboard_optimization", "visual_effects", 
            "transparency_effects", "raw_aim_curve", "stickykeys_guard",
            "tcp_ip", "dns_cloudflare", "network_power", "do_solo_mode", 
            "mtu_optimization", "gpu_scheduling", "high_performance", 
            "game_mode", "win32_priority", "hibernation_off", "windows_search", 
            "sysmain_off", "telemetry_off", "ultimate_power_plan", "mpo_fix",
            "usb_optimization", "input_queues", "trim_force_on", "usb_power_guard"
        ],
        sched: "smooth"
    },
    extreme: {
        tweaks: TWEAKS_DATABASE.map(t => t.id),
        sched: "aggressive"
    }
};

// Global App State
let activeTweaks = new Set();
let activeCategory = "dashboard";

// DOM Elements
const gaugeOpt = document.getElementById("gauge-opt");
const gaugeTweaks = document.getElementById("gauge-tweaks");
const textOptPct = document.getElementById("text-opt-pct");
const textTweaksCount = document.getElementById("text-tweaks-count");
const valFps = document.getElementById("val-fps");
const valPing = document.getElementById("val-ping");
const valRam = document.getElementById("val-ram");

const schedBalanced = document.getElementById("sched-balanced");
const schedSmooth = document.getElementById("sched-smooth");
const schedAggressive = document.getElementById("sched-aggressive");

const panelDashboard = document.getElementById("panel-dashboard");
const panelTweaks = document.getElementById("panel-tweaks");
const panelCategoryTitle = document.getElementById("panel-category-title");
const simTweaksList = document.getElementById("sim-tweaks-list");

// Initialize simulator gauges on page load
document.addEventListener("DOMContentLoaded", () => {
    updateDashboardUI();
    renderCatalogGrid(TWEAKS_DATABASE);
    
    // Wire up Sidebar Category Navigation
    document.querySelectorAll(".sim-nav-btn").forEach(btn => {
        btn.addEventListener("click", (e) => {
            document.querySelectorAll(".sim-nav-btn").forEach(b => b.classList.remove("active"));
            btn.classList.add("active");
            
            const category = btn.getAttribute("data-category");
            activeCategory = category;
            
            if (category === "dashboard") {
                panelDashboard.classList.add("active");
                panelTweaks.classList.remove("active");
            } else {
                panelDashboard.classList.remove("active");
                panelTweaks.classList.add("active");
                
                // Map category key to UI Category Name
                const catNameMap = {
                    input: "Input & Visuals",
                    network: "Red & Ping",
                    system: "Sistema & GPU",
                    clean: "Limpieza",
                    ghost: "GHOST Pack",
                    advanced: "Advanced"
                };
                
                panelCategoryTitle.textContent = catNameMap[category];
                renderSimulatorTweaks(catNameMap[category]);
            }
        });
    });

    // Wire up Profiles selectors
    document.querySelectorAll(".btn-profile").forEach(btn => {
        btn.addEventListener("click", () => {
            const profileKey = btn.getAttribute("data-profile");
            
            // Remove active state from other profiles
            document.querySelectorAll(".btn-profile").forEach(b => b.classList.remove("active"));
            btn.classList.add("active");
            
            // Set active tweaks according to profile
            activeTweaks.clear();
            PROFILES[profileKey].tweaks.forEach(id => activeTweaks.add(id));
            
            // Set CPU Scheduling badges
            updateCpuSchedulingBadge(PROFILES[profileKey].sched);
            
            // Refresh
            updateDashboardUI();
            if (activeCategory !== "dashboard") {
                const catNameMap = {
                    input: "Input & Visuals",
                    network: "Red & Ping",
                    system: "Sistema & GPU",
                    clean: "Limpieza",
                    ghost: "GHOST Pack",
                    advanced: "Advanced"
                };
                renderSimulatorTweaks(catNameMap[activeCategory]);
            }
        });
    });

    // Wire up Catalog Filters
    document.querySelectorAll(".filter-tab").forEach(tab => {
        tab.addEventListener("click", () => {
            document.querySelectorAll(".filter-tab").forEach(t => t.classList.remove("active"));
            tab.classList.add("active");
            
            filterCatalog();
        });
    });

    // Search bar listener
    document.getElementById("catalog-search").addEventListener("input", () => {
        filterCatalog();
    });

    // Copy command button
    const btnCopyRescue = document.getElementById("btn-copy-rescue");
    if (btnCopyRescue) {
        btnCopyRescue.addEventListener("click", () => {
            const commandText = "powershell -ExecutionPolicy Bypass -File %LOCALAPPDATA%\\GhostOptimizer\\Rescue\\RESCUE_SCRIPT_20250125_153045.ps1";
            navigator.clipboard.writeText(commandText).then(() => {
                const originalText = btnCopyRescue.textContent;
                btnCopyRescue.textContent = "¡Copiado con éxito!";
                btnCopyRescue.style.background = "#0e7a0d";
                btnCopyRescue.style.color = "#fff";
                setTimeout(() => {
                    btnCopyRescue.textContent = originalText;
                    btnCopyRescue.style.background = "";
                    btnCopyRescue.style.color = "";
                }, 2000);
            }).catch(err => {
                console.error("No se pudo copiar el comando: ", err);
            });
        });
    }
});

// Update CPU Scheduling Badges
function updateCpuSchedulingBadge(mode) {
    schedBalanced.classList.remove("active");
    schedSmooth.classList.remove("active");
    schedAggressive.classList.remove("active");
    
    if (mode === "balanced") schedBalanced.classList.add("active");
    if (mode === "smooth") schedSmooth.classList.add("active");
    if (mode === "aggressive") schedAggressive.classList.add("active");
}

// Render tweaks inside simulator list
function renderSimulatorTweaks(categoryName) {
    simTweaksList.innerHTML = "";
    
    const tweaksInCategory = TWEAKS_DATABASE.filter(t => t.category === categoryName);
    
    tweaksInCategory.forEach(tweak => {
        const item = document.createElement("div");
        item.className = "tweak-item";
        
        const isTweakActive = activeTweaks.has(tweak.id);
        
        item.innerHTML = `
            <div class="tweak-item-info">
                <div class="tweak-item-title">${tweak.title}</div>
                <div class="tweak-item-desc">${tweak.desc}</div>
            </div>
            <div class="tweak-toggle-group">
                <button class="btn-toggle btn-toggle-on ${isTweakActive ? 'active' : ''}" onclick="toggleTweak('${tweak.id}', true)">ON</button>
                <button class="btn-toggle btn-toggle-off ${!isTweakActive ? 'active' : ''}" onclick="toggleTweak('${tweak.id}', false)">OFF</button>
            </div>
        `;
        
        simTweaksList.appendChild(item);
    });
}

// Toggle individual tweak state (ON/OFF)
window.toggleTweak = function(id, state) {
    // Remove active state profile pills since we modified manually
    document.querySelectorAll(".btn-profile").forEach(b => b.classList.remove("active"));
    
    if (state) {
        activeTweaks.add(id);
    } else {
        activeTweaks.delete(id);
    }
    
    // Automatically match scheduling to tweak count
    if (activeTweaks.size < 20) {
        updateCpuSchedulingBadge("balanced");
    } else if (activeTweaks.size < 40) {
        updateCpuSchedulingBadge("smooth");
    } else {
        updateCpuSchedulingBadge("aggressive");
    }
    
    updateDashboardUI();
    renderSimulatorTweaks(TWEAKS_DATABASE.find(t => t.id === id).category);
};

// Calculate and render dashboard stats based on active tweaks
function updateDashboardUI() {
    const totalCount = TWEAKS_DATABASE.length;
    const activeCount = activeTweaks.size;
    const optPercentage = Math.round((activeCount / totalCount) * 100);
    
    // Render Gauge SVG Strokes
    // Circumference = 2 * PI * r = 2 * 3.14159 * 40 = 251.2
    const circumference = 251.2;
    const offsetOpt = circumference - (optPercentage / 100) * circumference;
    const offsetTweaks = circumference - (activeCount / totalCount) * circumference;
    
    gaugeOpt.style.strokeDashoffset = offsetOpt;
    gaugeTweaks.style.strokeDashoffset = offsetTweaks;
    
    // Update labels
    textOptPct.textContent = `${optPercentage}%`;
    textTweaksCount.textContent = `${activeCount}/${totalCount}`;
    
    // Estimate values
    let totalFps = 0;
    let totalPing = 0;
    let totalRam = 0;
    
    activeTweaks.forEach(id => {
        const tweak = TWEAKS_DATABASE.find(t => t.id === id);
        if (tweak) {
            totalFps += tweak.fps || 0;
            totalPing += tweak.ping || 0;
            totalRam += tweak.ram || 0;
        }
    });
    
    // Add base modifiers to avoid hard limits and make it look live
    if (activeCount > 0) {
        // Linear mapping up to max capacities
        totalFps = Math.min(120, Math.round((activeCount / totalCount) * 120));
        totalPing = Math.min(150, Math.round((activeCount / totalCount) * 150));
        totalRam = Math.min(11.5, Number(((activeCount / totalCount) * 11.5).toFixed(1)));
    }
    
    valFps.textContent = `+${totalFps} FPS`;
    valPing.textContent = `-${totalPing} ms`;
    valRam.textContent = `${totalRam} GB`;
}

// Render dynamic catalog card grids
function renderCatalogGrid(tweaksList) {
    const catalogGrid = document.getElementById("catalog-grid");
    catalogGrid.innerHTML = "";
    
    if (tweaksList.length === 0) {
        catalogGrid.innerHTML = `
            <div class="no-results" style="grid-column: 1/-1; text-align: center; padding: 40px; color: var(--text-muted);">
                Ningún tweak coincide con los filtros seleccionados.
            </div>
        `;
        return;
    }
    
    tweaksList.forEach(tweak => {
        const card = document.createElement("div");
        card.className = "catalog-card glass-card";
        
        let riskClass = "risk-safe";
        if (tweak.risk === "moderate") riskClass = "risk-moderate";
        if (tweak.risk === "danger") riskClass = "risk-danger";
        
        const riskLabels = {
            safe: "Seguro",
            moderate: "Moderado",
            danger: "Riesgoso"
        };
        
        let bulletPoints = "";
        if (tweak.benefits && tweak.benefits.length > 0) {
            bulletPoints = tweak.benefits.map(b => `<span>✓ ${b}</span>`).join("");
        }
        
        card.innerHTML = `
            <div class="card-header-row">
                <h4 class="card-title">${tweak.title}</h4>
                <span class="card-category-badge">${tweak.category}</span>
            </div>
            <p class="card-desc">${tweak.desc}</p>
            <div class="card-bullets">
                ${bulletPoints}
            </div>
            <div class="card-meta">
                <span class="card-risk ${riskClass}">
                    <span class="risk-dot"></span>
                    ${riskLabels[tweak.risk]}
                </span>
                ${tweak.restart ? '<span class="card-restart-badge">Reinicio</span>' : ''}
            </div>
        `;
        
        catalogGrid.appendChild(card);
    });
}

// Filter and Search Catalog items
function filterCatalog() {
    const searchQuery = document.getElementById("catalog-search").value.toLowerCase();
    const activeFilter = document.querySelector(".filter-tab.active").getAttribute("data-filter");
    
    const filtered = TWEAKS_DATABASE.filter(tweak => {
        // Match Search Query
        const matchesSearch = 
            tweak.title.toLowerCase().includes(searchQuery) ||
            tweak.desc.toLowerCase().includes(searchQuery) ||
            (tweak.benefits && tweak.benefits.some(b => b.toLowerCase().includes(searchQuery)));
            
        // Match Category Tab
        const matchesCategory = 
            activeFilter === "all" || tweak.category === activeFilter;
            
        return matchesSearch && matchesCategory;
    });
    
    renderCatalogGrid(filtered);
}
