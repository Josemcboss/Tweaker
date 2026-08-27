using System;
using System.Runtime.InteropServices;

public class CPUIDSDK
{
	private enum PTR : uint
	{
		PTR0 = 654986772u,
		PTR1 = 1930487329u,
		PTR2 = 1078886557u,
		PTR3 = 854353368u,
		PTR4 = 339093612u,
		PTR5 = 3618746211u,
		PTR6 = 2657303750u,
		PTR7 = 1754583337u,
		PTR8 = 2799127982u,
		PTR9 = 2609723162u,
		PTR10 = 1112048785u,
		PTR11 = 2377849718u,
		PTR12 = 1578548269u,
		PTR13 = 317597148u,
		PTR14 = 633752460u,
		PTR15 = 4202558715u,
		PTR16 = 3656627175u,
		PTR17 = 370027548u,
		PTR18 = 3984710403u,
		PTR19 = 4247911011u,
		PTR20 = 2745255746u,
		PTR21 = 4144754199u,
		PTR22 = 3854551935u,
		PTR23 = 1657718173u,
		PTR24 = 61081416u,
		PTR25 = 3605638611u,
		PTR26 = 3547965171u,
		PTR27 = 4120505139u,
		PTR28 = 1576844281u,
		PTR29 = 3416495943u,
		PTR30 = 2408783654u,
		PTR31 = 3833317623u,
		PTR32 = 1848171601u,
		PTR33 = 2439979742u,
		PTR34 = 2156069126u,
		PTR35 = 1308007405u,
		PTR36 = 2380864466u,
		PTR37 = 3379008207u,
		PTR38 = 421278264u,
		PTR39 = 952529292u,
		PTR40 = 1195282045u,
		PTR41 = 3123672158u,
		PTR42 = 987264432u,
		PTR43 = 1490858425u,
		PTR44 = 2385321050u,
		PTR45 = 128192328u,
		PTR46 = 935096184u,
		PTR47 = 3894792267u,
		PTR48 = 3756113859u,
		PTR49 = 926314092u,
		PTR50 = 3292498047u,
		PTR51 = 3674977815u,
		PTR52 = 2314277858u,
		PTR53 = 2768456198u,
		PTR54 = 1804916521u,
		PTR55 = 2121596137u,
		PTR56 = 1957488985u,
		PTR57 = 2196964838u,
		PTR58 = 690377292u,
		PTR59 = 3405878787u,
		PTR60 = 2384272442u,
		PTR61 = 1063943892u,
		PTR62 = 510278868u,
		PTR63 = 3736321383u,
		PTR64 = 875980908u,
		PTR65 = 1052802432u,
		PTR66 = 3204546050u,
		PTR67 = 2975687354u,
		PTR68 = 1436724037u,
		PTR69 = 3845245539u,
		PTR70 = 3665147115u,
		PTR71 = 3065212262u,
		PTR72 = 3581782779u,
		PTR73 = 1492562413u,
		PTR74 = 3162732806u,
		PTR75 = 1593884161u,
		PTR76 = 1055948256u,
		PTR77 = 345516336u,
		PTR78 = 1068138324u,
		PTR79 = 227547936u,
		PTR80 = 3833973003u,
		PTR81 = 1357029829u,
		PTR82 = 1306041265u,
		PTR83 = 3847867059u,
		PTR84 = 1678559257u,
		PTR85 = 3882995427u,
		PTR86 = 2833207742u,
		PTR87 = 4214748783u,
		PTR88 = 1091207701u,
		PTR89 = 2376670034u,
		PTR90 = 1665582733u,
		PTR91 = 2646031214u,
		PTR92 = 2271022778u,
		PTR93 = 702829512u,
		PTR94 = 227679012u,
		PTR95 = 1083212065u,
		PTR96 = 2376538958u,
		PTR97 = 1548531865u,
		PTR98 = 2030760469u,
		PTR99 = 551436732u,
		PTR100 = 2647079822u,
		PTR101 = 2237205170u,
		PTR102 = 156635820u,
		PTR103 = 2193425786u,
		PTR104 = 757619280u,
		PTR105 = 1777259485u,
		PTR106 = 4025343963u,
		PTR107 = 3248849739u,
		PTR108 = 57018060u,
		PTR109 = 560480976u,
		PTR110 = 980186328u,
		PTR111 = 4129811535u,
		PTR112 = 2896779602u,
		PTR113 = 1368826669u,
		PTR114 = 2666347994u,
		PTR115 = 1955391769u,
		PTR116 = 210376980u,
		PTR117 = 349710768u,
		PTR118 = 2115435565u,
		PTR119 = 4029145167u,
		PTR120 = 3121837094u,
		PTR121 = 4004896107u,
		PTR122 = 1600700113u,
		PTR123 = 33948684u,
		PTR124 = 3356463135u,
		PTR125 = 3955611531u,
		PTR126 = 3446774499u,
		PTR127 = 3830958255u,
		PTR128 = 2638559882u,
		PTR129 = 628902648u,
		PTR130 = 3000198566u,
		PTR131 = 3867135231u,
		PTR132 = 749361492u,
		PTR133 = 3858615291u,
		PTR134 = 4288544571u,
		PTR135 = 1164479185u,
		PTR136 = 136187964u,
		PTR137 = 4163629143u,
		PTR138 = 689197608u,
		PTR139 = 2113338349u,
		PTR140 = 946368720u,
		PTR141 = 1160153677u,
		PTR142 = 3414529803u,
		PTR143 = 3373371939u,
		PTR144 = 3804480903u,
		PTR145 = 2263027142u,
		PTR146 = 93195036u,
		PTR147 = 1968368293u,
		PTR148 = 724719204u,
		PTR149 = 3432618291u,
		PTR150 = 2485594190u,
		PTR151 = 1773589357u,
		PTR152 = 1830214189u,
		PTR153 = 1983835261u,
		PTR154 = 1721158957u,
		PTR155 = 2909625050u,
		PTR156 = 1454943601u,
		PTR157 = 2428182902u,
		PTR158 = 3754540947u,
		PTR159 = 4095993927u,
		PTR160 = 2989188182u,
		PTR161 = 669405132u,
		PTR162 = 390344328u,
		PTR163 = 345123108u,
		PTR164 = 1409722381u,
		PTR165 = 1406052253u,
		PTR166 = 2615883734u,
		PTR167 = 726423192u,
		PTR168 = 1784337589u,
		PTR169 = 2851558382u,
		PTR170 = 2643933998u,
		PTR171 = 1704119077u,
		PTR172 = 3181345598u,
		PTR173 = 248389020u,
		PTR174 = 2575774478u,
		PTR175 = 1577237509u,
		PTR176 = 3523060731u,
		PTR177 = 1196592805u,
		PTR178 = 1754059033u,
		PTR179 = 2153709758u,
		PTR180 = 3820472175u,
		PTR181 = 2165506598u,
		PTR182 = 4152618759u,
		PTR183 = 3667637559u,
		PTR184 = 534134700u,
		PTR185 = 3118822346u,
		PTR186 = 149557716u,
		PTR187 = 851994000u,
		PTR188 = 2674212554u,
		PTR189 = 1408149469u,
		PTR190 = 2070214345u,
		PTR191 = 146018664u,
		PTR192 = 2226719090u,
		PTR193 = 4153929519u,
		PTR194 = 3459488871u,
		PTR195 = 2261323154u,
		PTR196 = 3159324830u,
		PTR197 = 2139553549u,
		PTR198 = 360590076u,
		PTR199 = 3430258923u,
		PTR200 = 339486840u,
		PTR201 = 948597012u,
		PTR202 = 1611579421u,
		PTR203 = 873490464u,
		PTR204 = 1618788601u,
		PTR205 = 1006401528u,
		PTR206 = 2195260850u,
		PTR207 = 1098416881u,
		PTR208 = 3426719871u,
		PTR209 = 3633033495u,
		PTR210 = 1694812681u,
		PTR211 = 3135206846u,
		PTR212 = 1253479789u,
		PTR213 = 2915916698u,
		PTR214 = 361245456u,
		PTR215 = 918711684u,
		PTR216 = 842294376u,
		PTR217 = 707286096u,
		PTR218 = 2078472133u,
		PTR219 = 2317292606u,
		PTR220 = 3130094882u,
		PTR221 = 2522557622u,
		PTR222 = 723015216u,
		PTR223 = 3040176746u,
		PTR224 = 4124044191u,
		PTR225 = 4258528167u,
		PTR226 = 3165485402u,
		PTR227 = 3738680751u,
		PTR228 = 1412212825u,
		PTR229 = 2745517898u,
		PTR230 = 1438034797u,
		PTR231 = 1442098153u,
		PTR232 = 592332444u,
		PTR233 = 4202951943u,
		PTR234 = 3464076531u,
		PTR235 = 2694136106u,
		PTR236 = 4293525459u,
		PTR237 = 607668336u,
		PTR238 = 1466347213u,
		PTR239 = 2370378386u,
		PTR240 = 3638800839u,
		PTR241 = 2831503754u,
		PTR242 = 857368116u,
		PTR243 = 847406340u,
		PTR244 = 820011456u,
		PTR245 = 2070345421u,
		PTR246 = 2262109610u,
		PTR247 = 1044020340u,
		PTR248 = 3614945007u,
		PTR249 = 524435076u,
		PTR250 = 2972541530u,
		PTR251 = 2983027610u,
		PTR252 = 1428073021u,
		PTR253 = 3190127690u,
		PTR254 = 1135773541u,
		PTR255 = 2502765146u,
		PTR256 = 1260426817u,
		PTR257 = 2780121962u,
		PTR258 = 530726724u,
		PTR259 = 3876048399u,
		PTR260 = 3858746367u,
		PTR261 = 2420973722u,
		PTR262 = 244718892u,
		PTR263 = 253107756u,
		PTR264 = 2404851374u,
		PTR265 = 3658724391u,
		PTR266 = 3324087363u,
		PTR267 = 3369832887u,
		PTR268 = 3224207451u,
		PTR269 = 539377740u,
		PTR270 = 2670018122u,
		PTR271 = 1246663837u,
		PTR272 = 2657434826u,
		PTR273 = 2791263422u,
		PTR274 = 818700696u,
		PTR275 = 3208478330u,
		PTR276 = 2382175226u,
		PTR277 = 1922229541u,
		PTR278 = 1277335621u,
		PTR279 = 1943463853u,
		PTR280 = 3547834095u,
		PTR281 = 1256756689u,
		PTR282 = 565199712u,
		PTR283 = 2521377938u,
		PTR284 = 107351244u,
		PTR285 = 2781170570u,
		PTR286 = 2275348286u,
		PTR287 = 3660821607u,
		PTR288 = 335030256u,
		PTR289 = 3212934914u,
		PTR290 = 3871722891u,
		PTR291 = 2464884182u,
		PTR292 = 4162449459u,
		PTR293 = 765352764u,
		PTR294 = 2111503285u,
		PTR295 = 2369198702u,
		PTR296 = 780164352u,
		PTR297 = 3840264651u,
		PTR298 = 4196529219u,
		PTR299 = 2906610302u,
		PTR300 = 2824818878u,
		PTR301 = 3694901367u,
		PTR302 = 3767517471u,
		PTR303 = 4215797391u,
		PTR304 = 4081968795u,
		PTR305 = 3335228823u,
		PTR306 = 110759220u,
		PTR307 = 1614332017u,
		PTR308 = 631524168u,
		PTR309 = 3935687979u,
		PTR310 = 2283737150u,
		PTR311 = 3280176903u,
		PTR312 = 3690182631u,
		PTR313 = 1615118473u,
		PTR314 = 2878428962u,
		PTR315 = 1670170393u,
		PTR316 = 3984579327u,
		PTR317 = 1965615697u,
		PTR318 = 2935184870u,
		PTR319 = 1503441721u,
		PTR320 = 2256473342u,
		PTR321 = 3226697895u,
		PTR322 = 4054049607u,
		PTR323 = 462173976u,
		PTR324 = 205133940u,
		PTR325 = 164762532u,
		PTR326 = 3947222667u,
		PTR327 = 1691273629u,
		PTR328 = 789470748u,
		PTR329 = 2221344974u,
		PTR330 = 2031678001u,
		PTR331 = 4289068875u,
		PTR332 = 1786303729u,
		PTR333 = 2175075146u,
		PTR334 = 1508422609u,
		PTR335 = 1879367689u,
		PTR336 = 3535381875u,
		PTR337 = 814244112u,
		PTR338 = 1269077833u,
		PTR339 = 3389756439u,
		PTR340 = 3213852446u,
		PTR341 = 2726380802u,
		PTR342 = 769940424u,
		PTR343 = 1022523876u,
		PTR344 = 1045200024u,
		PTR345 = 3053415422u,
		PTR346 = 916221240u,
		PTR347 = 3044633330u,
		PTR348 = 3459357795u,
		PTR349 = 1380099205u,
		PTR350 = 3422263287u,
		PTR351 = 2948030318u,
		PTR352 = 2244414350u,
		PTR353 = 176690448u,
		PTR354 = 1997336089u,
		PTR355 = 1149929749u,
		PTR356 = 516308364u,
		PTR357 = 3606949371u,
		PTR358 = 3673667055u,
		PTR359 = 1904403205u,
		PTR360 = 2298286586u,
		PTR361 = 1451928853u,
		PTR362 = 3158931602u,
		PTR363 = 3363672315u,
		PTR364 = 2665954766u,
		PTR365 = 2907789986u,
		PTR366 = 3139270202u,
		PTR367 = 3317664639u,
		PTR368 = 3872116119u,
		PTR369 = 830497536u,
		PTR370 = 965505816u,
		PTR371 = 1564523137u,
		PTR372 = 90966744u,
		PTR373 = 1831262797u,
		PTR374 = 1077837949u,
		PTR375 = 3877097007u,
		PTR376 = 1607122837u,
		PTR377 = 3648500463u,
		PTR378 = 1406445481u,
		PTR379 = 2626500890u,
		PTR380 = 3374027319u,
		PTR381 = 1199214325u,
		PTR382 = 1083736369u,
		PTR383 = 3741957651u
	}

	private delegate IntPtr CPUIDSDK_fp_CreateInstance();

	private delegate void CPUIDSDK_fp_DestroyInstance(IntPtr objptr);

	private delegate int CPUIDSDK_fp_Init(IntPtr objptr, string _szDllPath, string _szDllFilename, int _config_flag, ref int _errorcode, ref int _extended_errorcode);

	private delegate void CPUIDSDK_fp_Close(IntPtr objptr);

	private delegate void CPUIDSDK_fp_RefreshInformation(IntPtr objptr);

	private delegate void CPUIDSDK_fp_GetDllVersion(IntPtr objptr, ref int _version);

	private delegate int CPUIDSDK_fp_GetNbProcessors(IntPtr objptr);

	private delegate int CPUIDSDK_fp_GetProcessorFamily(IntPtr objptr, int _proc_index);

	private delegate int CPUIDSDK_fp_GetProcessorCoreSetCount(IntPtr objptr, int _proc_index);

	private delegate int CPUIDSDK_fp_GetProcessorCoreCount(IntPtr objptr, int _proc_index, int _core_set);

	private delegate int CPUIDSDK_fp_GetProcessorThreadCount(IntPtr objptr, int _proc_index, int _core_set);

	private delegate int CPUIDSDK_fp_GetProcessorCoreSetType(IntPtr objptr, int _proc_index, int _core_set);

	private delegate int CPUIDSDK_fp_GetProcessorCoreThreadCount(IntPtr objptr, int _proc_index, int _core_index);

	private delegate int CPUIDSDK_fp_GetProcessorThreadAPICID(IntPtr objptr, int _proc_index, int _core_index, int _thread_index);

	private delegate IntPtr CPUIDSDK_fp_GetProcessorName(IntPtr objptr, int _proc_index);

	private delegate IntPtr CPUIDSDK_fp_GetProcessorCodeName(IntPtr objptr, int _proc_index);

	private delegate IntPtr CPUIDSDK_fp_GetProcessorSpecification(IntPtr objptr, int _proc_index);

	private delegate IntPtr CPUIDSDK_fp_GetProcessorPackage(IntPtr objptr, int _proc_index);

	private delegate IntPtr CPUIDSDK_fp_GetProcessorStepping(IntPtr objptr, int _proc_index);

	private delegate float CPUIDSDK_fp_GetProcessorTDP(IntPtr objptr, int _proc_index);

	private delegate float CPUIDSDK_fp_GetProcessorManufacturingProcess(IntPtr objptr, int _proc_index);

	private delegate int CPUIDSDK_fp_IsProcessorInstructionSetAvailable(IntPtr objptr, int _proc_index, int _iset);

	private delegate float CPUIDSDK_fp_GetProcessorCoreClockFrequency(IntPtr objptr, int _proc_index, int _core_index);

	private delegate float CPUIDSDK_fp_GetProcessorCoreClockMultiplier(IntPtr objptr, int _proc_index, int _core_index);

	private delegate float CPUIDSDK_fp_GetProcessorCoreTemperature(IntPtr objptr, int _proc_index, int _core_index);

	private delegate float CPUIDSDK_fp_GetBusFrequency(IntPtr objptr);

	private delegate float CPUIDSDK_fp_GetProcessorRatedBusFrequency(IntPtr objptr, int _proc_index);

	private delegate int CPUIDSDK_fp_GetProcessorStockClockFrequency(IntPtr objptr, int _proc_index);

	private delegate int CPUIDSDK_fp_GetProcessorStockBusFrequency(IntPtr objptr, int _proc_index);

	private delegate int CPUIDSDK_fp_GetProcessorMaxCacheLevel(IntPtr objptr, int _proc_index);

	private delegate void CPUIDSDK_fp_GetProcessorCacheParameters(IntPtr objptr, int _proc_index, int _core_set, int _cache_level, int _cache_type, ref int _NbCaches, ref int _size);

	private delegate int CPUIDSDK_fp_GetProcessorID(IntPtr objptr, int _proc_index);

	private delegate float CPUIDSDK_fp_GetProcessorVoltageID(IntPtr objptr, int _proc_index);

	private delegate int CPUIDSDK_fp_GetMemoryType(IntPtr objptr);

	private delegate int CPUIDSDK_fp_GetMemorySize(IntPtr objptr);

	private delegate float CPUIDSDK_fp_GetMemoryClockFrequency(IntPtr objptr);

	private delegate int CPUIDSDK_fp_GetMemoryNumberOfChannels(IntPtr objptr);

	private delegate float CPUIDSDK_fp_GetMemoryCASLatency(IntPtr objptr);

	private delegate int CPUIDSDK_fp_GetMemoryRAStoCASDelay(IntPtr objptr);

	private delegate int CPUIDSDK_fp_GetMemoryRASPrecharge(IntPtr objptr);

	private delegate int CPUIDSDK_fp_GetMemoryTRAS(IntPtr objptr);

	private delegate int CPUIDSDK_fp_GetMemoryTRC(IntPtr objptr);

	private delegate int CPUIDSDK_fp_GetMemoryCommandRate(IntPtr objptr);

	private delegate IntPtr CPUIDSDK_fp_GetNorthBridgeVendor(IntPtr objptr);

	private delegate IntPtr CPUIDSDK_fp_GetNorthBridgeModel(IntPtr objptr);

	private delegate IntPtr CPUIDSDK_fp_GetNorthBridgeRevision(IntPtr objptr);

	private delegate IntPtr CPUIDSDK_fp_GetSouthBridgeVendor(IntPtr objptr);

	private delegate IntPtr CPUIDSDK_fp_GetSouthBridgeModel(IntPtr objptr);

	private delegate IntPtr CPUIDSDK_fp_GetSouthBridgeRevision(IntPtr objptr);

	private delegate void CPUIDSDK_fp_GetGraphicBusLinkParameters(IntPtr objptr, ref int _bus_type, ref int _link_width);

	private delegate void CPUIDSDK_fp_GetMemorySlotsConfig(IntPtr objptr, ref int _nbslots, ref int _nbusedslots, ref int _slotmap_h, ref int _slotmap_l, ref int _maxmodulesize);

	private delegate IntPtr CPUIDSDK_fp_GetBIOSVendor(IntPtr objptr);

	private delegate IntPtr CPUIDSDK_fp_GetBIOSVersion(IntPtr objptr);

	private delegate IntPtr CPUIDSDK_fp_GetBIOSDate(IntPtr objptr);

	private delegate IntPtr CPUIDSDK_fp_GetMainboardVendor(IntPtr objptr);

	private delegate IntPtr CPUIDSDK_fp_GetMainboardModel(IntPtr objptr);

	private delegate IntPtr CPUIDSDK_fp_GetMainboardRevision(IntPtr objptr);

	private delegate IntPtr CPUIDSDK_fp_GetMainboardSerialNumber(IntPtr objptr);

	private delegate IntPtr CPUIDSDK_fp_GetSystemManufacturer(IntPtr objptr);

	private delegate IntPtr CPUIDSDK_fp_GetSystemProductName(IntPtr objptr);

	private delegate IntPtr CPUIDSDK_fp_GetSystemVersion(IntPtr objptr);

	private delegate IntPtr CPUIDSDK_fp_GetSystemSerialNumber(IntPtr objptr);

	private delegate IntPtr CPUIDSDK_fp_GetSystemUUID(IntPtr objptr);

	private delegate IntPtr CPUIDSDK_fp_GetSystemSKU(IntPtr objptr);

	private delegate IntPtr CPUIDSDK_fp_GetSystemFamily(IntPtr objptr);

	private delegate IntPtr CPUIDSDK_fp_GetChassisManufacturer(IntPtr objptr);

	private delegate IntPtr CPUIDSDK_fp_GetChassisType(IntPtr objptr);

	private delegate IntPtr CPUIDSDK_fp_GetChassisSerialNumber(IntPtr objptr);

	private delegate int CPUIDSDK_fp_GetMemoryInfosExt(IntPtr objptr, ref IntPtr _szLocation, ref IntPtr _szUsage, ref IntPtr _szCorrection);

	private delegate int CPUIDSDK_fp_GetNumberOfMemoryDevices(IntPtr objptr);

	private delegate int CPUIDSDK_fp_GetMemoryDeviceInfos(IntPtr objptr, int _device_index, ref int _size, ref IntPtr _szFormat);

	private delegate int CPUIDSDK_fp_GetMemoryDeviceInfosExt(IntPtr objptr, int _device_index, ref IntPtr _szDesignation, ref IntPtr _szType, ref int _total_width, ref int _data_width, ref int _speed);

	private delegate int CPUIDSDK_fp_GetProcessorSockets(IntPtr objptr);

	private delegate int CPUIDSDK_fp_GetMemoryMaxCapacity(IntPtr objptr);

	private delegate int CPUIDSDK_fp_GetMemoryMaxNumberOfDevices(IntPtr objptr);

	private delegate int CPUIDSDK_fp_GetNumberOfSPDModules(IntPtr objptr);

	private delegate int CPUIDSDK_fp_GetSPDModuleType(IntPtr objptr, int _spd_index);

	private delegate int CPUIDSDK_fp_GetSPDModuleSize(IntPtr objptr, int _spd_index);

	private delegate IntPtr CPUIDSDK_fp_GetSPDModuleFormat(IntPtr objptr, int _spd_index);

	private delegate IntPtr CPUIDSDK_fp_GetSPDModuleManufacturer(IntPtr objptr, int _spd_index);

	private delegate int CPUIDSDK_fp_GetSPDModuleManufacturerID(IntPtr objptr, int _spd_index, byte[] _id);

	private delegate IntPtr CPUIDSDK_fp_GetSPDModuleDRAMManufacturer(IntPtr objptr, int _spd_index);

	private delegate int CPUIDSDK_fp_GetSPDModuleMaxFrequency(IntPtr objptr, int _spd_index);

	private delegate IntPtr CPUIDSDK_fp_GetSPDModuleSpecification(IntPtr objptr, int _spd_index);

	private delegate IntPtr CPUIDSDK_fp_GetSPDModulePartNumber(IntPtr objptr, int _spd_index);

	private delegate IntPtr CPUIDSDK_fp_GetSPDModuleSerialNumber(IntPtr objptr, int _spd_index);

	private delegate float CPUIDSDK_fp_GetSPDModuleMinTRCD(IntPtr objptr, int _spd_index);

	private delegate float CPUIDSDK_fp_GetSPDModuleMinTRP(IntPtr objptr, int _spd_index);

	private delegate float CPUIDSDK_fp_GetSPDModuleMinTRAS(IntPtr objptr, int _spd_index);

	private delegate float CPUIDSDK_fp_GetSPDModuleMinTRC(IntPtr objptr, int _spd_index);

	private delegate int CPUIDSDK_fp_GetSPDModuleManufacturingDate(IntPtr objptr, int _spd_index, ref int _year, ref int _week);

	private delegate int CPUIDSDK_fp_GetSPDModuleNumberOfBanks(IntPtr objptr, int _spd_index);

	private delegate int CPUIDSDK_fp_GetSPDModuleDataWidth(IntPtr objptr, int _spd_index);

	private delegate float CPUIDSDK_fp_GetSPDModuleTemperature(IntPtr objptr, int _spd_index);

	private delegate int CPUIDSDK_fp_GetSPDModuleNumberOfProfiles(IntPtr objptr, int _spd_index);

	private delegate void CPUIDSDK_fp_GetSPDModuleProfileInfos(IntPtr objptr, int _spd_index, int _profile_index, ref float _frequency, ref float _tCL, ref float _nominal_vdd);

	private delegate int CPUIDSDK_fp_GetSPDModuleNumberOfEPPProfiles(IntPtr objptr, int _spd_index, ref int _epp_revision);

	private delegate void CPUIDSDK_fp_GetSPDModuleEPPProfileInfos(IntPtr objptr, int _spd_index, int _profile_index, ref float _frequency, ref float _tCL, ref float _tRCD, ref float _tRAS, ref float _tRP, ref float _tRC, ref float _nominal_vdd);

	private delegate int CPUIDSDK_fp_GetSPDModuleNumberOfXMPProfiles(IntPtr objptr, int _spd_index, ref int _xmp_revision);

	private delegate int CPUIDSDK_fp_GetSPDModuleXMPProfileNumberOfCL(IntPtr objptr, int _spd_index, int _profile_index);

	private delegate void CPUIDSDK_fp_GetSPDModuleXMPProfileCLInfos(IntPtr objptr, int _spd_index, int _profile_index, int _cl_index, ref float _frequency, ref float _CL);

	private delegate void CPUIDSDK_fp_GetSPDModuleXMPProfileInfos(IntPtr objptr, int _spd_index, int _profile_index, ref float _tRCD, ref float _tRAS, ref float _tRP, ref float _tRC, ref float _nominal_vdd, ref int _max_freq, ref float _max_CL);

	private delegate int CPUIDSDK_fp_GetSPDModuleNumberOfAMPProfiles(IntPtr objptr, int _spd_index, ref int _amp_revision);

	private delegate void CPUIDSDK_fp_GetSPDModuleAMPProfileInfos(IntPtr objptr, int _spd_index, int _profile_index, ref int _frequency, ref float _min_cycle_time, ref float _tCL, ref float _tRCD, ref float _tRAS, ref float _tRP, ref float _tRC);

	private delegate int CPUIDSDK_fp_GetSPDModuleRawData(IntPtr objptr, int _spd_index, int _offset);

	private delegate int CPUIDSDK_fp_GetNumberOfDisplayAdapter(IntPtr objptr);

	private delegate int CPUIDSDK_fp_GetDisplayAdapterID(IntPtr objptr, int _adapter_index);

	private delegate IntPtr CPUIDSDK_fp_GetDisplayAdapterName(IntPtr objptr, int _adapter_index);

	private delegate IntPtr CPUIDSDK_fp_GetDisplayAdapterCodeName(IntPtr objptr, int _adapter_index);

	private delegate int CPUIDSDK_fp_GetDisplayAdapterNumberOfPerformanceLevels(IntPtr objptr, int _adapter_index);

	private delegate int CPUIDSDK_fp_GetDisplayAdapterCurrentPerformanceLevel(IntPtr objptr, int _adapter_index);

	private delegate IntPtr CPUIDSDK_fp_GetDisplayAdapterPerformanceLevelName(IntPtr objptr, int _adapter_index, int _perf_level);

	private delegate float CPUIDSDK_fp_GetDisplayAdapterClock(IntPtr objptr, int _perf_level, int _adapter_index, int _domain);

	private delegate float CPUIDSDK_fp_GetDisplayAdapterStockClock(IntPtr objptr, int _perf_level, int _adapter_index, int _domain);

	private delegate float CPUIDSDK_fp_GetDisplayAdapterTemperature(IntPtr objptr, int _adapter_index, int _domain);

	private delegate int CPUIDSDK_fp_GetDisplayAdapterFanSpeed(IntPtr objptr, int _adapter_index);

	private delegate int CPUIDSDK_fp_GetDisplayAdapterFanPWM(IntPtr objptr, int _adapter_index);

	private delegate int CPUIDSDK_fp_GetDisplayAdapterMemoryType(IntPtr objptr, int _adapter_index, ref int _type);

	private delegate int CPUIDSDK_fp_GetDisplayAdapterMemorySize(IntPtr objptr, int _adapter_index, ref int _size);

	private delegate int CPUIDSDK_fp_GetDisplayAdapterMemoryBusWidth(IntPtr objptr, int _adapter_index, ref int _bus_width);

	private delegate IntPtr CPUIDSDK_fp_GetDisplayAdapterMemoryVendor(IntPtr objptr, int _adapter_index);

	private delegate IntPtr CPUIDSDK_fp_GetDirectXVersion(IntPtr objptr);

	private delegate int CPUIDSDK_fp_GetDisplayAdapterBusInfos(IntPtr objptr, int _adapter_index, ref int _bus_type, ref int _multi_vpu);

	private delegate float CPUIDSDK_fp_GetDisplayAdapterManufacturingProcess(IntPtr objptr, int _adapter_index);

	private delegate IntPtr CPUIDSDK_fp_GetDisplayAdapterCoreFamily(IntPtr objptr, int _adapter_index, ref int _core);

	private delegate IntPtr CPUIDSDK_fp_GetDisplayAdapterDriverVersion(IntPtr objptr, int _adapter_index);

	private delegate int CPUIDSDK_fp_GetDisplayAdapterPCIAddress(IntPtr objptr, int _adapter_index, ref int _bus, ref int _device, ref int _function);

	private delegate int CPUIDSDK_fp_GetNumberOfMonitors(IntPtr objptr);

	private delegate IntPtr CPUIDSDK_fp_GetMonitorName(IntPtr objptr, int _monitor_index);

	private delegate IntPtr CPUIDSDK_fp_GetMonitorVendor(IntPtr objptr, int _monitor_index);

	private delegate IntPtr CPUIDSDK_fp_GetMonitorID(IntPtr objptr, int _monitor_index);

	private delegate IntPtr CPUIDSDK_fp_GetMonitorSerial(IntPtr objptr, int _monitor_index);

	private delegate int CPUIDSDK_fp_GetMonitorManufacturingDate(IntPtr objptr, int _monitor_index, ref int _week, ref int _year);

	private delegate float CPUIDSDK_fp_GetMonitorSize(IntPtr objptr, int _monitor_index);

	private delegate int CPUIDSDK_fp_GetMonitorResolution(IntPtr objptr, int _monitor_index, ref int _width, ref int _height, ref int _frequency);

	private delegate int CPUIDSDK_fp_GetMonitorMaxPixelClock(IntPtr objptr, int _monitor_index);

	private delegate float CPUIDSDK_fp_GetMonitorGamma(IntPtr objptr, int _monitor_index);

	private delegate int CPUIDSDK_fp_GetNumberOfStorageDevice(IntPtr objptr);

	private delegate int CPUIDSDK_fp_GetStorageDriveNumber(IntPtr objptr, int _index);

	private delegate IntPtr CPUIDSDK_fp_GetStorageDeviceName(IntPtr objptr, int _index);

	private delegate IntPtr CPUIDSDK_fp_GetStorageDeviceRevision(IntPtr objptr, int _index);

	private delegate IntPtr CPUIDSDK_fp_GetStorageDeviceSerialNumber(IntPtr objptr, int _index);

	private delegate int CPUIDSDK_fp_GetStorageDeviceBusType(IntPtr objptr, int _index);

	private delegate int CPUIDSDK_fp_GetStorageDeviceRotationSpeed(IntPtr objptr, int _index);

	private delegate int CPUIDSDK_fp_GetStorageDeviceFeatureFlag(IntPtr objptr, int _index);

	private delegate int CPUIDSDK_fp_GetStorageDeviceNumberOfVolumes(IntPtr objptr, int _index);

	private delegate IntPtr CPUIDSDK_fp_GetStorageDeviceVolumeLetter(IntPtr objptr, int _index, int _volume_index);

	private delegate float CPUIDSDK_fp_GetStorageDeviceVolumeTotalCapacity(IntPtr objptr, int _index, int _volume_index);

	private delegate float CPUIDSDK_fp_GetStorageDeviceVolumeAvailableCapacity(IntPtr objptr, int _index, int _volume_index);

	private delegate int CPUIDSDK_fp_GetStorageDeviceSmartAttribute(IntPtr objptr, int _index, int _attrib_index, ref int _id, ref int _flags, ref int _value, ref int _worst, byte[] _data);

	private delegate int CPUIDSDK_fp_GetStorageDevicePowerOnHours(IntPtr objptr, int _index);

	private delegate int CPUIDSDK_fp_GetStorageDevicePowerCycleCount(IntPtr objptr, int _index);

	private delegate float CPUIDSDK_fp_GetStorageDeviceTotalCapacity(IntPtr objptr, int _index);

	private delegate int CPUIDSDK_fp_GetNumberOfDevices(IntPtr objptr);

	private delegate int CPUIDSDK_fp_GetDeviceClass(IntPtr objptr, int _device_index);

	private delegate IntPtr CPUIDSDK_fp_GetDeviceName(IntPtr objptr, int _device_index);

	private delegate IntPtr CPUIDSDK_fp_GetDeviceSerialNumber(IntPtr objptr, int _device_index);

	private delegate int CPUIDSDK_fp_GetNumberOfSensors(IntPtr objptr, int _device_index, int _sensor_class);

	private delegate int CPUIDSDK_fp_GetSensorInfos(IntPtr objptr, int _device_index, int _sensor_index, int _sensor_class, ref int _sensor_id, ref IntPtr _szNamePtr, ref int _raw_value, ref float _value, ref float _min_value, ref float _max_value);

	private delegate void CPUIDSDK_fp_SensorClearMinMax(IntPtr objptr, int _device_index, int _sensor_index, int _sensor_class);

	private delegate float CPUIDSDK_fp_GetSensorTypeValue(IntPtr objptr, int _sensor_type, ref int _device_index, ref int _sensor_index);

	public const uint CPUIDSDK_ERROR_NO_ERROR = 0u;

	public const uint CPUIDSDK_ERROR_EVALUATION = 1u;

	public const uint CPUIDSDK_ERROR_DRIVER = 2u;

	public const uint CPUIDSDK_ERROR_VM_RUNNING = 4u;

	public const uint CPUIDSDK_ERROR_LOCKED = 8u;

	public const uint CPUIDSDK_ERROR_INVALID_DLL = 16u;

	public const uint CPUIDSDK_EXT_ERROR_EVAL_1 = 1u;

	public const uint CPUIDSDK_EXT_ERROR_EVAL_2 = 2u;

	public const uint CPUIDSDK_CONFIG_USE_SOFTWARE = 2u;

	public const uint CPUIDSDK_CONFIG_USE_DMI = 4u;

	public const uint CPUIDSDK_CONFIG_USE_PCI = 8u;

	public const uint CPUIDSDK_CONFIG_USE_ACPI = 16u;

	public const uint CPUIDSDK_CONFIG_USE_CHIPSET = 32u;

	public const uint CPUIDSDK_CONFIG_USE_SMBUS = 64u;

	public const uint CPUIDSDK_CONFIG_USE_SPD = 128u;

	public const uint CPUIDSDK_CONFIG_USE_STORAGE = 256u;

	public const uint CPUIDSDK_CONFIG_USE_GRAPHICS = 512u;

	public const uint CPUIDSDK_CONFIG_USE_HWMONITORING = 1024u;

	public const uint CPUIDSDK_CONFIG_USE_PROCESSOR = 2048u;

	public const uint CPUIDSDK_CONFIG_USE_DISPLAY_API = 4096u;

	public const uint CPUIDSDK_CONFIG_USE_ACPI_TIMER = 16384u;

	public const uint CPUIDSDK_CONFIG_UPDATE_PROCESSOR = 65536u;

	public const uint CPUIDSDK_CONFIG_UPDATE_GRAPHICS = 131072u;

	public const uint CPUIDSDK_CONFIG_UPDATE_STORAGE = 262144u;

	public const uint CPUIDSDK_CONFIG_UPDATE_LPCIO = 524288u;

	public const uint CPUIDSDK_CONFIG_UPDATE_DRAM = 1048576u;

	public const uint CPUIDSDK_CONFIG_CHECK_VM = 16777216u;

	public const uint CPUIDSDK_CONFIG_WAKEUP_HDD = 33554432u;

	public const uint CPUIDSDK_CONFIG_SCAN_USB_HDD = 67108864u;

	public const uint CPUIDSDK_CONFIG_USE_USBXPRESS = 134217728u;

	public const uint CPUIDSDK_CONFIG_USE_LPCIO = 268435456u;

	public const uint CPUIDSDK_CONFIG_SERVER_SAFE = 2147483648u;

	public const uint CPUIDSDK_CONFIG_USE_EVERYTHING = 2147483647u;

	public static int I_UNDEFINED_VALUE = -1;

	public static float F_UNDEFINED_VALUE = -1f;

	public static uint MAX_INTEGER = uint.MaxValue;

	public static float MAX_FLOAT = MAX_INTEGER;

	public const uint CLASS_DEVICE_UNKNOWN = 0u;

	public const uint CLASS_DEVICE_PCI = 1u;

	public const uint CLASS_DEVICE_SMBUS = 2u;

	public const uint CLASS_DEVICE_PROCESSOR = 4u;

	public const uint CLASS_DEVICE_LPCIO = 8u;

	public const uint CLASS_DEVICE_DRIVE = 16u;

	public const uint CLASS_DEVICE_DISPLAY_ADAPTER = 32u;

	public const uint CLASS_DEVICE_HID = 64u;

	public const uint CLASS_DEVICE_BATTERY = 128u;

	public const uint CLASS_DEVICE_EVBOT = 256u;

	public const uint CLASS_DEVICE_NETWORK = 512u;

	public const uint CLASS_DEVICE_MAINBOARD = 1024u;

	public const uint CLASS_DEVICE_MEMORY_MODULE = 2048u;

	public const uint CLASS_DEVICE_PSU = 4096u;

	public const uint CLASS_DEVICE_TYPE_MASK = 2147483647u;

	public const uint CLASS_DEVICE_COMPOSITE = 2147483648u;

	public const uint CPU_MANUFACTURER_MASK = 4278190080u;

	public const uint CPU_FAMILY_MASK = 4294967040u;

	public const uint CPU_MODEL_MASK = uint.MaxValue;

	public const uint CPU_UNKNOWN = 0u;

	public const uint CPU_INTEL = 16777216u;

	public const uint CPU_AMD = 33554432u;

	public const uint CPU_CYRIX = 67108864u;

	public const uint CPU_VIA = 134217728u;

	public const uint CPU_TRANSMETA = 268435456u;

	public const uint CPU_DMP = 536870912u;

	public const uint CPU_UMC = 1073741824u;

	public const uint CPU_IBM = 2181038080u;

	public const uint CPU_QUALCOMM = 2197815296u;

	public const uint CPU_HYGON = 2214592512u;

	public const uint CPU_INTEL_386 = 16777472u;

	public const uint CPU_INTEL_486 = 16777728u;

	public const uint CPU_INTEL_P5 = 16778240u;

	public const uint CPU_INTEL_P6 = 16779264u;

	public const uint CPU_INTEL_NETBURST = 16781312u;

	public const uint CPU_INTEL_MOBILE = 16785408u;

	public const uint CPU_INTEL_CORE = 16793600u;

	public const uint CPU_INTEL_CORE_2 = 16809984u;

	public const uint CPU_INTEL_BONNELL = 16842752u;

	public const uint CPU_INTEL_SALTWELL = 16843008u;

	public const uint CPU_INTEL_SILVERMONT = 16843264u;

	public const uint CPU_INTEL_GOLDMONT = 16843776u;

	public const uint CPU_INTEL_NEHALEM = 16908288u;

	public const uint CPU_INTEL_SANDY_BRIDGE = 16908544u;

	public const uint CPU_INTEL_HASWELL = 16908800u;

	public const uint CPU_INTEL_SKYLAKE = 17039360u;

	public const uint CPU_INTEL_ITANIUM = 17825792u;

	public const uint CPU_INTEL_ITANIUM_2 = 17826048u;

	public const uint CPU_INTEL_MIC = 18874368u;

	public const uint CPU_PENTIUM = 16778241u;

	public const uint CPU_PENTIUM_MMX = 16778242u;

	public const uint CPU_PENTIUM_PRO = 16779265u;

	public const uint CPU_PENTIUM_2 = 16779266u;

	public const uint CPU_PENTIUM_2_M = 16779267u;

	public const uint CPU_CELERON_P2 = 16779268u;

	public const uint CPU_XEON_P2 = 16779269u;

	public const uint CPU_PENTIUM_3 = 16779270u;

	public const uint CPU_PENTIUM_3_M = 16779271u;

	public const uint CPU_PENTIUM_3_S = 16779272u;

	public const uint CPU_CELERON_P3 = 16779273u;

	public const uint CPU_XEON_P3 = 16779274u;

	public const uint CPU_PENTIUM_4 = 16781313u;

	public const uint CPU_PENTIUM_4_M = 16781314u;

	public const uint CPU_PENTIUM_4_HT = 16781315u;

	public const uint CPU_PENTIUM_4_EE = 16781316u;

	public const uint CPU_CELERON_P4 = 16781317u;

	public const uint CPU_CELERON_D = 16781318u;

	public const uint CPU_XEON_P4 = 16781319u;

	public const uint CPU_PENTIUM_D = 16781320u;

	public const uint CPU_PENTIUM_XE = 16781321u;

	public const uint CPU_PENTIUM_M = 16785409u;

	public const uint CPU_CELERON_M = 16785410u;

	public const uint CPU_CORE_SOLO = 16793601u;

	public const uint CPU_CORE_DUO = 16793602u;

	public const uint CPU_CORE_CELERON_M = 16793603u;

	public const uint CPU_CORE_CELERON = 16793604u;

	public const uint CPU_CORE_2_DUO = 16809985u;

	public const uint CPU_CORE_2_EE = 16809986u;

	public const uint CPU_CORE_2_XEON = 16809987u;

	public const uint CPU_CORE_2_CELERON = 16809988u;

	public const uint CPU_CORE_2_QUAD = 16809989u;

	public const uint CPU_CORE_2_PENTIUM = 16809990u;

	public const uint CPU_CORE_2_CELERON_DC = 16809991u;

	public const uint CPU_CORE_2_SOLO = 16809992u;

	public const uint CPU_BONNELL_ATOM = 16842753u;

	public const uint CPU_SALTWELL_ATOM = 16843009u;

	public const uint CPU_SILVERMONT_ATOM = 16843265u;

	public const uint CPU_SILVERMONT_CELERON = 16843266u;

	public const uint CPU_SILVERMONT_PENTIUM = 16843267u;

	public const uint CPU_SILVERMONT_ATOM_X7 = 16843268u;

	public const uint CPU_SILVERMONT_ATOM_X5 = 16843269u;

	public const uint CPU_SILVERMONT_ATOM_X3 = 16843270u;

	public const uint CPU_GOLDMONT_ATOM = 16843777u;

	public const uint CPU_GOLDMONT_CELERON = 16843778u;

	public const uint CPU_GOLDMONT_PENTIUM = 16843779u;

	public const uint CPU_NEHALEM_CORE_I7 = 16908289u;

	public const uint CPU_NEHALEM_CORE_I7E = 16908290u;

	public const uint CPU_NEHALEM_XEON = 16908291u;

	public const uint CPU_NEHALEM_CORE_I3 = 16908292u;

	public const uint CPU_NEHALEM_CORE_I5 = 16908293u;

	public const uint CPU_NEHALEM_PENTIUM = 16908295u;

	public const uint CPU_NEHALEM_CELERON = 16908296u;

	public const uint CPU_SANDY_BRIDGE_CORE_I7 = 16908545u;

	public const uint CPU_SANDY_BRIDGE_CORE_I7E = 16908546u;

	public const uint CPU_SANDY_BRIDGE_XEON = 16908547u;

	public const uint CPU_SANDY_BRIDGE_CORE_I3 = 16908548u;

	public const uint CPU_SANDY_BRIDGE_CORE_I5 = 16908549u;

	public const uint CPU_SANDY_BRIDGE_PENTIUM = 16908551u;

	public const uint CPU_SANDY_BRIDGE_CELERON = 16908552u;

	public const uint CPU_HASWELL_CORE_I7 = 16908801u;

	public const uint CPU_HASWELL_CORE_I7E = 16908802u;

	public const uint CPU_HASWELL_XEON = 16908803u;

	public const uint CPU_HASWELL_CORE_I3 = 16908804u;

	public const uint CPU_HASWELL_CORE_I5 = 16908805u;

	public const uint CPU_HASWELL_PENTIUM = 16908807u;

	public const uint CPU_HASWELL_CELERON = 16908808u;

	public const uint CPU_HASWELL_CORE_M = 16908809u;

	public const uint CPU_SKYLAKE_XEON = 17039361u;

	public const uint CPU_SKYLAKE_CORE_I7 = 17039362u;

	public const uint CPU_SKYLAKE_CORE_I5 = 17039363u;

	public const uint CPU_SKYLAKE_CORE_I3 = 17039364u;

	public const uint CPU_SKYLAKE_PENTIUM = 17039365u;

	public const uint CPU_SKYLAKE_CELERON = 17039366u;

	public const uint CPU_SKYLAKE_CORE_M7 = 17039367u;

	public const uint CPU_SKYLAKE_CORE_M5 = 17039368u;

	public const uint CPU_SKYLAKE_CORE_M3 = 17039369u;

	public const uint CPU_SKYLAKE_CORE_I9EX = 17039370u;

	public const uint CPU_SKYLAKE_CORE_I9X = 17039371u;

	public const uint CPU_SKYLAKE_CORE_I7X = 17039372u;

	public const uint CPU_SKYLAKE_CORE_I5X = 17039373u;

	public const uint CPU_SKYLAKE_XEON_BRONZE = 17039374u;

	public const uint CPU_SKYLAKE_XEON_SILVER = 17039375u;

	public const uint CPU_SKYLAKE_XEON_GOLD = 17039376u;

	public const uint CPU_SKYLAKE_XEON_PLATINIUM = 17039377u;

	public const uint CPU_SKYLAKE_PENTIUM_GOLD = 17039378u;

	public const uint CPU_SKYLAKE_CORE_I9_GEN9 = 17039392u;

	public const uint CPU_SKYLAKE_CORE_I7_GEN9 = 17039393u;

	public const uint CPU_SKYLAKE_CORE_I5_GEN9 = 17039394u;

	public const uint CPU_SKYLAKE_CORE_I3_GEN9 = 17039395u;

	public const uint CPU_SKYLAKE_CORE_I9_GEN10 = 17039396u;

	public const uint CPU_SKYLAKE_CORE_I7_GEN10 = 17039397u;

	public const uint CPU_SKYLAKE_CORE_I5_GEN10 = 17039398u;

	public const uint CPU_SKYLAKE_CORE_I3_GEN10 = 17039399u;

	public const uint CPU_SKYLAKE_CORE_I9_GEN11 = 17039400u;

	public const uint CPU_SKYLAKE_CORE_I7_GEN11 = 17039401u;

	public const uint CPU_SKYLAKE_CORE_I5_GEN11 = 17039402u;

	public const uint CPU_SKYLAKE_CORE_I3_GEN11 = 17039403u;

	public const uint CPU_SKYLAKE_CORE_I9_GEN12 = 17039404u;

	public const uint CPU_SKYLAKE_CORE_I7_GEN12 = 17039405u;

	public const uint CPU_SKYLAKE_CORE_I5_GEN12 = 17039406u;

	public const uint CPU_SKYLAKE_CORE_I3_GEN12 = 17039407u;

	public const uint CPU_SKYLAKE_CELERON_GEN12 = 17039408u;

	public const uint CPU_SKYLAKE_PENTIUM_GEN12 = 17039409u;

	public const uint CPU_SKYLAKE_XEON_GEN10 = 17039410u;

	public const uint CPU_SKYLAKE_XEON_SILVER_GEN10 = 17039411u;

	public const uint CPU_SKYLAKE_XEON_GOLD_GEN10 = 17039412u;

	public const uint CPU_SKYLAKE_XEON_PLATINUM_GEN10 = 17039413u;

	public const uint CPU_SKYLAKE_CORE_N_GEN12 = 17039414u;

	public const uint CPU_INTEL_CORE_GEN15 = 17039415u;

	public const uint CPU_INTEL_CORE_3_GEN15 = 17039416u;

	public const uint CPU_INTEL_CORE_5_GEN15 = 17039417u;

	public const uint CPU_INTEL_CORE_7_GEN15 = 17039418u;

	public const uint CPU_INTEL_CORE_ULTRA_GEN15 = 17039425u;

	public const uint CPU_INTEL_CORE_ULTRA_5_GEN15 = 17039426u;

	public const uint CPU_INTEL_CORE_ULTRA_7_GEN15 = 17039427u;

	public const uint CPU_INTEL_CORE_ULTRA_9_GEN15 = 17039428u;

	public const uint CPU_AMD_386 = 33554688u;

	public const uint CPU_AMD_486 = 33554944u;

	public const uint CPU_AMD_K5 = 33555456u;

	public const uint CPU_AMD_K6 = 33556480u;

	public const uint CPU_AMD_K7 = 33558528u;

	public const uint CPU_AMD_K8 = 33562624u;

	public const uint CPU_AMD_K10 = 33570816u;

	public const uint CPU_AMD_K12 = 33619968u;

	public const uint CPU_AMD_K14 = 33685504u;

	public const uint CPU_AMD_K15 = 33816576u;

	public const uint CPU_AMD_K16 = 34078720u;

	public const uint CPU_AMD_K17 = 34603008u;

	public const uint CPU_K5 = 33555457u;

	public const uint CPU_K5_GEODE = 33555458u;

	public const uint CPU_K6 = 33556481u;

	public const uint CPU_K6_2 = 33556482u;

	public const uint CPU_K6_3 = 33556483u;

	public const uint CPU_K7_ATHLON = 33558529u;

	public const uint CPU_K7_ATHLON_XP = 33558530u;

	public const uint CPU_K7_ATHLON_MP = 33558531u;

	public const uint CPU_K7_DURON = 33558532u;

	public const uint CPU_K7_SEMPRON = 33558533u;

	public const uint CPU_K7_SEMPRON_M = 33558534u;

	public const uint CPU_K8_ATHLON_64 = 33562625u;

	public const uint CPU_K8_ATHLON_64_M = 33562626u;

	public const uint CPU_K8_ATHLON_64_FX = 33562627u;

	public const uint CPU_K8_OPTERON = 33562628u;

	public const uint CPU_K8_TURION_64 = 33562629u;

	public const uint CPU_K8_SEMPRON = 33562630u;

	public const uint CPU_K8_SEMPRON_M = 33562631u;

	public const uint CPU_K8_ATHLON_64_X2 = 33562632u;

	public const uint CPU_K8_TURION_64_X2 = 33562633u;

	public const uint CPU_K8_ATHLON_NEO = 33562634u;

	public const uint CPU_K10_PHENOM = 33570817u;

	public const uint CPU_K10_PHENOM_X3 = 33570818u;

	public const uint CPU_K10_PHENOM_FX = 33570819u;

	public const uint CPU_K10_OPTERON = 33570820u;

	public const uint CPU_K10_TURION_64 = 33570821u;

	public const uint CPU_K10_TURION_64_ULTRA = 33570822u;

	public const uint CPU_K10_ATHLON_64 = 33570823u;

	public const uint CPU_K10_SEMPRON = 33570824u;

	public const uint CPU_K10_ATHLON_2 = 33570833u;

	public const uint CPU_K10_ATHLON_2_X2 = 33570827u;

	public const uint CPU_K10_ATHLON_2_X3 = 33570829u;

	public const uint CPU_K10_ATHLON_2_X4 = 33570828u;

	public const uint CPU_K10_PHENOM_II = 33570825u;

	public const uint CPU_K10_PHENOM_II_X2 = 33570826u;

	public const uint CPU_K10_PHENOM_II_X3 = 33570830u;

	public const uint CPU_K10_PHENOM_II_X4 = 33570831u;

	public const uint CPU_K10_PHENOM_II_X6 = 33570832u;

	public const uint CPU_K15_FXB = 33816577u;

	public const uint CPU_K15_OPTERON = 33816578u;

	public const uint CPU_K15_A10T = 33816579u;

	public const uint CPU_K15_A8T = 33816580u;

	public const uint CPU_K15_A6T = 33816581u;

	public const uint CPU_K15_A4T = 33816582u;

	public const uint CPU_K15_ATHLON_X4 = 33816583u;

	public const uint CPU_K15_FXV = 33816584u;

	public const uint CPU_K15_A10R = 33816585u;

	public const uint CPU_K15_A8R = 33816586u;

	public const uint CPU_K15_A6R = 33816587u;

	public const uint CPU_K15_A4R = 33816588u;

	public const uint CPU_K15_SEMPRON = 33816589u;

	public const uint CPU_K15_ATHLON_X2 = 33816590u;

	public const uint CPU_K15_FXC = 33816591u;

	public const uint CPU_K15_A10C = 33816592u;

	public const uint CPU_K15_A8C = 33816593u;

	public const uint CPU_K15_A6C = 33816594u;

	public const uint CPU_K15_A4C = 33816595u;

	public const uint CPU_K15_A12 = 33816596u;

	public const uint CPU_K15_RX = 33816597u;

	public const uint CPU_K15_GX = 33816598u;

	public const uint CPU_K15_A9 = 33816599u;

	public const uint CPU_K15_E2 = 33816600u;

	public const uint CPU_K16_A6 = 34078721u;

	public const uint CPU_K16_A4 = 34078722u;

	public const uint CPU_K16_OPTERON = 34078725u;

	public const uint CPU_K16_ATHLON = 34078726u;

	public const uint CPU_K16_SEMPRON = 34078727u;

	public const uint CPU_K16_E1 = 34078728u;

	public const uint CPU_K16_E2 = 34078729u;

	public const uint CPU_K16_A8 = 34078730u;

	public const uint CPU_K16_A10 = 34078731u;

	public const uint CPU_K16_GX = 34078732u;

	public const uint CPU_RYZEN = 34603009u;

	public const uint CPU_RYZEN_7 = 34603010u;

	public const uint CPU_RYZEN_5 = 34603011u;

	public const uint CPU_RYZEN_3 = 34603012u;

	public const uint CPU_RYZEN_TR = 34603013u;

	public const uint CPU_RYZEN_EPYC = 34603014u;

	public const uint CPU_RYZEN_M = 34603015u;

	public const uint CPU_RYZEN_7_M = 34603016u;

	public const uint CPU_RYZEN_5_M = 34603017u;

	public const uint CPU_RYZEN_3_M = 34603018u;

	public const uint CPU_RYZEN_ATHLON = 34603019u;

	public const uint CPU_RYZEN_9 = 34603020u;

	public const uint CPU_RYZEN_9_M = 34603021u;

	public const uint CPU_RYZEN_Z1 = 34603022u;

	public const uint CPU_RYZEN_Z1_EXTREME = 34603023u;

	public const uint CPU_CX486 = 67109888u;

	public const uint CPU_CX5X86 = 67110144u;

	public const uint CPU_CX6X86 = 67110400u;

	public const uint CPU_VIA_WINCHIP = 134218752u;

	public const uint CPU_VIA_C3 = 134219776u;

	public const uint CPU_VIA_C7 = 134221824u;

	public const uint CPU_VIA_NANO = 134225920u;

	public const uint CPU_VIA_CHA = 134234112u;

	public const uint CPU_ZHAOXIN_KX = 134488064u;

	public const uint CPU_C3 = 134219777u;

	public const uint CPU_C7 = 134221825u;

	public const uint CPU_C7_M = 134221826u;

	public const uint CPU_EDEN = 134221827u;

	public const uint CPU_C7_D = 134221828u;

	public const uint CPU_NANO_X2 = 134225921u;

	public const uint CPU_EDEN_X2 = 134225922u;

	public const uint CPU_NANO_X3 = 134225923u;

	public const uint CPU_EDEN_X4 = 134225924u;

	public const uint CPU_QUADCORE = 134225925u;

	public const uint CPU_CX6X86L = 67110401u;

	public const uint CPU_MEDIAGX = 67110402u;

	public const uint CPU_CX6X86MX = 67110403u;

	public const uint CPU_MII = 67110404u;

	public const uint CPU_CRUSOE = 268435457u;

	public const uint CPU_EFFICEON = 268435458u;

	public const uint CPU_VORTEX86_SX = 536870913u;

	public const uint CPU_VORTEX86_EX = 536870914u;

	public const uint CPU_VORTEX86_DX = 536870915u;

	public const uint CPU_VORTEX86_MX = 536870916u;

	public const uint CPU_VORTEX86_DX3 = 536870917u;

	public const uint CPU_HYGON_C86 = 2214592768u;

	public const int HYBRID_CORE_TYPE_UNKNOWN = 0;

	public const int HYBRID_CORE_TYPE_QUARK = 16;

	public const int HYBRID_CORE_TYPE_ATOM = 32;

	public const int HYBRID_CORE_TYPE_KNIGHTS = 48;

	public const int HYBRID_CORE_TYPE_CORE = 64;

	public const int CACHE_TYPE_DATA = 1;

	public const int CACHE_TYPE_INSTRUCTION = 2;

	public const int CACHE_TYPE_UNIFIED = 3;

	public const int CACHE_TYPE_TRACE_CACHE = 4;

	public const int ISET_MMX = 1;

	public const int ISET_EXTENDED_MMX = 2;

	public const int ISET_3DNOW = 3;

	public const int ISET_EXTENDED_3DNOW = 4;

	public const int ISET_SSE = 5;

	public const int ISET_SSE2 = 6;

	public const int ISET_SSE3 = 7;

	public const int ISET_SSSE3 = 8;

	public const int ISET_SSE4_1 = 9;

	public const int ISET_SSE4_2 = 12;

	public const int ISET_SSE4A = 13;

	public const int ISET_XOP = 14;

	public const int ISET_X86_64 = 16;

	public const int ISET_NX = 17;

	public const int ISET_VMX = 18;

	public const int ISET_AES = 19;

	public const int ISET_AVX = 20;

	public const int ISET_AVX2 = 21;

	public const int ISET_FMA3 = 22;

	public const int ISET_FMA4 = 23;

	public const int ISET_RTM = 24;

	public const int ISET_HLE = 25;

	public const int ISET_AVX512F = 26;

	public const int ISET_SHA = 27;

	public const int HWM_CLASS_LPC = 1;

	public const int HWM_CLASS_CPU = 2;

	public const int HWM_CLASS_HDD = 4;

	public const int HWM_CLASS_DISPLAYADAPTER = 8;

	public const int HWM_CLASS_PSU = 16;

	public const int HWM_CLASS_ACPI = 32;

	public const int HWM_CLASS_RAM = 64;

	public const int HWM_CLASS_CHASSIS = 128;

	public const int HWM_CLASS_WATERCOOLER = 256;

	public const int HWM_CLASS_BATTERY = 512;

	public const int SENSOR_CLASS_VOLTAGE = 4096;

	public const int SENSOR_CLASS_TEMPERATURE = 8192;

	public const int SENSOR_CLASS_FAN = 12288;

	public const int SENSOR_CLASS_CURRENT = 16384;

	public const int SENSOR_CLASS_POWER = 20480;

	public const int SENSOR_CLASS_FAN_PWM = 24576;

	public const int SENSOR_CLASS_PUMP_PWM = 28672;

	public const int SENSOR_CLASS_WATER_LEVEL = 32768;

	public const int SENSOR_CLASS_POSITION = 36864;

	public const int SENSOR_CLASS_CAPACITY = 40960;

	public const int SENSOR_CLASS_CASEOPEN = 45056;

	public const int SENSOR_CLASS_LEVEL = 49152;

	public const int SENSOR_CLASS_COUNTER = 53248;

	public const int SENSOR_CLASS_UTILIZATION = 57344;

	public const int SENSOR_CLASS_CLOCK_SPEED = 61440;

	public const int SENSOR_CLASS_BANDWIDTH = 65536;

	public const int SENSOR_CLASS_PERF_LIMITER = 69632;

	public const int SENSOR_VOLTAGE_VCORE = 4198400;

	public const int SENSOR_VOLTAGE_3V3 = 8392704;

	public const int SENSOR_VOLTAGE_P5V = 12587008;

	public const int SENSOR_VOLTAGE_P12V = 16781312;

	public const int SENSOR_VOLTAGE_M5V = 20975616;

	public const int SENSOR_VOLTAGE_M12V = 25169920;

	public const int SENSOR_VOLTAGE_5VSB = 29364224;

	public const int SENSOR_VOLTAGE_DRAM = 33558528;

	public const int SENSOR_VOLTAGE_CPU_VTT = 37752832;

	public const int SENSOR_VOLTAGE_IOH_VCORE = 41947136;

	public const int SENSOR_VOLTAGE_IOH_PLL = 46141440;

	public const int SENSOR_VOLTAGE_CPU_PLL = 50335744;

	public const int SENSOR_VOLTAGE_PCH = 54530048;

	public const int SENSOR_VOLTAGE_CPU_VID = 58724352;

	public const int SENSOR_VOLTAGE_MAX_CPU_VID = 62918656;

	public const int SENSOR_VOLTAGE_MAX_CORESET_CPU_VID = 67112960;

	public const int SENSOR_VOLTAGE_GPU = 71307264;

	public const int SENSOR_TEMPERATURE_CPU = 4202496;

	public const int SENSOR_TEMPERATURE_VREG = 8396800;

	public const int SENSOR_TEMPERATURE_DRAM = 12591104;

	public const int SENSOR_TEMPERATURE_PCH = 16785408;

	public const int SENSOR_TEMPERATURE_CPU_PACKAGE = 25174016;

	public const int SENSOR_TEMPERATURE_CPU_NODE = 29368320;

	public const int SENSOR_TEMPERATURE_CPU_CCD = 33562624;

	public const int SENSOR_TEMPERATURE_CPU_CORE = 37756928;

	public const int SENSOR_TEMPERATURE_MAX_CPU_CORE = 41951232;

	public const int SENSOR_TEMPERATURE_MAX_CORESET_CPU_CORE = 46145536;

	public const int SENSOR_TEMPERATURE_GPU = 67117056;

	public const int SENSOR_TEMPERATURE_GPU_HOTSPOT = 71311360;

	public const int SENSOR_FAN_CPU = 4206592;

	public const int SENSOR_FAN_PUMP = 8400896;

	public const int SENSOR_FAN_GPU = 25178112;

	public const int SENSOR_POWER_CPU_PACKAGE = 4214784;

	public const int SENSOR_POWER_CPU_ALL_CORES = 8409088;

	public const int SENSOR_POWER_CPU_CORE = 12603392;

	public const int SENSOR_POWER_CPU_GT = 20992000;

	public const int SENSOR_POWER_GPU = 29380608;

	public const int SENSOR_UTILIZATION_CPU_PACKAGE = 4251648;

	public const int SENSOR_UTILIZATION_CPU_CORESET = 8445952;

	public const int SENSOR_UTILIZATION_CPU_CCX = 12640256;

	public const int SENSOR_UTILIZATION_CPU_CORE = 16834560;

	public const int SENSOR_UTILIZATION_CPU_THREAD = 21028864;

	public const int SENSOR_UTILIZATION_GPU = 25223168;

	public const int SENSOR_UTILIZATION_MEMORY = 29417472;

	public const int MEMORY_TYPE_SPM_RAM = 1;

	public const int MEMORY_TYPE_RDRAM = 2;

	public const int MEMORY_TYPE_EDO_RAM = 3;

	public const int MEMORY_TYPE_FPM_RAM = 4;

	public const int MEMORY_TYPE_SDRAM = 5;

	public const int MEMORY_TYPE_DDR_SDRAM = 6;

	public const int MEMORY_TYPE_DDR2_SDRAM = 7;

	public const int MEMORY_TYPE_DDR2_SDRAM_FB = 8;

	public const int MEMORY_TYPE_DDR3_SDRAM = 9;

	public const int MEMORY_TYPE_DDR4_SDRAM = 10;

	public const int MEMORY_TYPE_DDR5_SDRAM = 11;

	public const int DISPLAY_CLOCK_DOMAIN_GRAPHICS = 0;

	public const int DISPLAY_CLOCK_DOMAIN_MEMORY = 1;

	public const int DISPLAY_CLOCK_DOMAIN_PROCESSOR = 2;

	public const int MEMORY_TYPE_SDR = 1;

	public const int MEMORY_TYPE_DDR = 2;

	public const int MEMORY_TYPE_LPDDR2 = 9;

	public const int MEMORY_TYPE_DDR2 = 3;

	public const int MEMORY_TYPE_DDR3 = 7;

	public const int MEMORY_TYPE_GDDR2 = 4;

	public const int MEMORY_TYPE_GDDR3 = 5;

	public const int MEMORY_TYPE_GDDR4 = 6;

	public const int MEMORY_TYPE_GDDR5 = 8;

	public const int MEMORY_TYPE_GDDR5X = 10;

	public const int MEMORY_TYPE_HBM1 = 11;

	public const int MEMORY_TYPE_HBM2 = 12;

	public const int MEMORY_TYPE_SDDR4 = 13;

	public const int MEMORY_TYPE_GDDR6 = 14;

	public const int MEMORY_TYPE_GDDR6X = 15;

	public const int GRAPHIC_BUS_ISA = 0;

	public const int GRAPHIC_BUS_VLB = 1;

	public const int GRAPHIC_BUS_PCI = 2;

	public const int GRAPHIC_BUS_AGP = 3;

	public const int GRAPHIC_BUS_PCIE = 4;

	public const int DRIVE_FEATURE_IS_SSD = 1;

	public const int DRIVE_FEATURE_SMART = 2;

	public const int DRIVE_FEATURE_TRIM = 4;

	public const int DRIVE_FEATURE_IS_REMOVABLE = 16;

	public const int BUS_TYPE_SCSI = 1;

	public const int BUS_TYPE_ATAPI = 2;

	public const int BUS_TYPE_ATA = 3;

	public const int BUS_TYPE_IEEE1394 = 4;

	public const int BUS_TYPE_SSA = 5;

	public const int BUS_TYPE_FIBRE = 6;

	public const int BUS_TYPE_USB = 7;

	public const int BUS_TYPE_RAID = 8;

	public const int BUS_TYPE_ISCSI = 9;

	public const int BUS_TYPE_SAS = 10;

	public const int BUS_TYPE_SATA = 11;

	public const int BUS_TYPE_SD = 12;

	public const int BUS_TYPE_MMC = 13;

	public const int BUS_TYPE_VIRTUAL = 14;

	public const int BUS_TYPE_FILEBACKEDVIRTUAL = 15;

	public const int BUS_TYPE_SPACES = 16;

	public const int BUS_TYPE_NVME = 17;

	public const string szDllPath = "";

	public const string szDllFilename = "cpuidsdk.dll";

	protected const string szDllName = "cpuidsdk.dll";

	protected IntPtr objptr = IntPtr.Zero;

	public bool IS_F_DEFINED(float _f)
	{
		if (!(_f > 0f))
		{
			return false;
		}
		return true;
	}

	public bool IS_F_DEFINED(double _f)
	{
		if (!(_f > 0.0))
		{
			return false;
		}
		return true;
	}

	public bool IS_I_DEFINED(int _i)
	{
		if (_i != I_UNDEFINED_VALUE)
		{
			return true;
		}
		return false;
	}

	public bool IS_I_DEFINED(uint _i)
	{
		if (_i != (uint)I_UNDEFINED_VALUE)
		{
			return true;
		}
		return false;
	}

	public bool IS_I_DEFINED(short _i)
	{
		if (_i != (short)I_UNDEFINED_VALUE)
		{
			return true;
		}
		return false;
	}

	public bool IS_I_DEFINED(ushort _i)
	{
		if (_i != (ushort)I_UNDEFINED_VALUE)
		{
			return true;
		}
		return false;
	}

	public bool IS_I_DEFINED(sbyte _i)
	{
		if (_i != (sbyte)I_UNDEFINED_VALUE)
		{
			return true;
		}
		return false;
	}

	public bool IS_I_DEFINED(byte _i)
	{
		if (_i != (byte)I_UNDEFINED_VALUE)
		{
			return true;
		}
		return false;
	}

	[DllImport("cpuidsdk.dll", EntryPoint = "QueryInterface")]
	protected static extern IntPtr CPUIDSDK_fp_QueryInterface(uint _code);

	public bool CreateInstance()
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(654986772u);
			if (intPtr != IntPtr.Zero)
			{
				CPUIDSDK_fp_CreateInstance cPUIDSDK_fp_CreateInstance = (CPUIDSDK_fp_CreateInstance)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_CreateInstance));
				objptr = cPUIDSDK_fp_CreateInstance();
				if (objptr != IntPtr.Zero)
				{
					return true;
				}
				return false;
			}
			objptr = IntPtr.Zero;
			return false;
		}
		catch (Exception ex)
		{
			_ = ex.Message;
			return false;
		}
	}

	public void DestroyInstance()
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(1930487329u);
			if (intPtr != IntPtr.Zero)
			{
				((CPUIDSDK_fp_DestroyInstance)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_DestroyInstance)))(objptr);
			}
		}
		catch
		{
		}
	}

	public bool Init(string _szDllPath, string _szDllFilename, uint _config_flag, ref int _errorcode, ref int _extended_errorcode)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(1078886557u);
			if (intPtr != IntPtr.Zero)
			{
				if (((CPUIDSDK_fp_Init)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_Init)))(objptr, _szDllPath, _szDllFilename, (int)_config_flag, ref _errorcode, ref _extended_errorcode) == 1)
				{
					return true;
				}
				return false;
			}
			_errorcode = 16;
			return false;
		}
		catch
		{
			return false;
		}
	}

	public void Close()
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(854353368u);
			if (intPtr != IntPtr.Zero)
			{
				((CPUIDSDK_fp_Close)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_Close)))(objptr);
			}
		}
		catch
		{
		}
	}

	public void RefreshInformation()
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(339093612u);
			if (intPtr != IntPtr.Zero)
			{
				((CPUIDSDK_fp_RefreshInformation)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_RefreshInformation)))(objptr);
			}
		}
		catch
		{
		}
	}

	public void GetDllVersion(ref int _version)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(3618746211u);
			if (intPtr != IntPtr.Zero)
			{
				((CPUIDSDK_fp_GetDllVersion)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetDllVersion)))(objptr, ref _version);
			}
		}
		catch
		{
		}
	}

	public int GetNumberOfProcessors()
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(2799127982u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetNbProcessors)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetNbProcessors)))(objptr);
			}
			return I_UNDEFINED_VALUE;
		}
		catch
		{
			return I_UNDEFINED_VALUE;
		}
	}

	public int GetProcessorFamily(int _proc_index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(2609723162u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetProcessorFamily)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetProcessorFamily)))(objptr, _proc_index);
			}
			return I_UNDEFINED_VALUE;
		}
		catch
		{
			return I_UNDEFINED_VALUE;
		}
	}

	public int GetProcessorCoreSetCount(int _proc_index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(3756113859u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetProcessorCoreSetCount)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetProcessorCoreSetCount)))(objptr, _proc_index);
			}
			return I_UNDEFINED_VALUE;
		}
		catch
		{
			return I_UNDEFINED_VALUE;
		}
	}

	public int GetProcessorCoreCount(int _proc_index, int _core_set)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(633752460u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetProcessorCoreCount)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetProcessorCoreCount)))(objptr, _proc_index, _core_set);
			}
			return I_UNDEFINED_VALUE;
		}
		catch
		{
			return I_UNDEFINED_VALUE;
		}
	}

	public int GetProcessorThreadCount(int _proc_index, int _core_set)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(4202558715u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetProcessorThreadCount)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetProcessorThreadCount)))(objptr, _proc_index, _core_set);
			}
			return I_UNDEFINED_VALUE;
		}
		catch
		{
			return I_UNDEFINED_VALUE;
		}
	}

	public int GetProcessorCoreSetType(int _proc_index, int _core_set)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(926314092u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetProcessorCoreSetType)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetProcessorCoreSetType)))(objptr, _proc_index, _core_set);
			}
			return I_UNDEFINED_VALUE;
		}
		catch
		{
			return I_UNDEFINED_VALUE;
		}
	}

	public int GetProcessorCoreThreadCount(int _proc_index, int _core_index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(3656627175u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetProcessorCoreThreadCount)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetProcessorCoreThreadCount)))(objptr, _proc_index, _core_index);
			}
			return I_UNDEFINED_VALUE;
		}
		catch
		{
			return I_UNDEFINED_VALUE;
		}
	}

	public int GetProcessorThreadAPICID(IntPtr objptr, int _proc_index, int _core_index, int _thread_index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(370027548u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetProcessorThreadAPICID)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetProcessorThreadAPICID)))(objptr, _proc_index, _core_index, _thread_index);
			}
			return I_UNDEFINED_VALUE;
		}
		catch
		{
			return I_UNDEFINED_VALUE;
		}
	}

	public string GetProcessorName(int _proc_index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(3984710403u);
			if (intPtr != IntPtr.Zero)
			{
				IntPtr ptr = ((CPUIDSDK_fp_GetProcessorName)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetProcessorName)))(objptr, _proc_index);
				string result = Marshal.PtrToStringAnsi(ptr);
				Marshal.FreeBSTR(ptr);
				return result;
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	public string GetProcessorCodeName(int _proc_index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(4247911011u);
			if (intPtr != IntPtr.Zero)
			{
				IntPtr ptr = ((CPUIDSDK_fp_GetProcessorCodeName)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetProcessorCodeName)))(objptr, _proc_index);
				string result = Marshal.PtrToStringAnsi(ptr);
				Marshal.FreeBSTR(ptr);
				return result;
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	public string GetProcessorSpecification(int _proc_index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(2745255746u);
			if (intPtr != IntPtr.Zero)
			{
				return Marshal.PtrToStringAnsi(((CPUIDSDK_fp_GetProcessorSpecification)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetProcessorSpecification)))(objptr, _proc_index));
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	public string GetProcessorPackage(int _proc_index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(4144754199u);
			if (intPtr != IntPtr.Zero)
			{
				IntPtr ptr = ((CPUIDSDK_fp_GetProcessorPackage)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetProcessorPackage)))(objptr, _proc_index);
				string result = Marshal.PtrToStringAnsi(ptr);
				Marshal.FreeBSTR(ptr);
				return result;
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	public string GetProcessorStepping(int _proc_index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(3854551935u);
			if (intPtr != IntPtr.Zero)
			{
				IntPtr ptr = ((CPUIDSDK_fp_GetProcessorStepping)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetProcessorStepping)))(objptr, _proc_index);
				string result = Marshal.PtrToStringAnsi(ptr);
				Marshal.FreeBSTR(ptr);
				return result;
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	public float GetProcessorTDP(int _proc_index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(1657718173u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetProcessorTDP)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetProcessorTDP)))(objptr, _proc_index);
			}
			return F_UNDEFINED_VALUE;
		}
		catch
		{
			return F_UNDEFINED_VALUE;
		}
	}

	public float GetProcessorManufacturingProcess(int _proc_index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(61081416u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetProcessorManufacturingProcess)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetProcessorManufacturingProcess)))(objptr, _proc_index);
			}
			return F_UNDEFINED_VALUE;
		}
		catch
		{
			return F_UNDEFINED_VALUE;
		}
	}

	public bool IsProcessorInstructionSetAvailable(int _proc_index, int _iset)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(2439979742u);
			if (intPtr != IntPtr.Zero)
			{
				if (((CPUIDSDK_fp_IsProcessorInstructionSetAvailable)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_IsProcessorInstructionSetAvailable)))(objptr, _proc_index, _iset) == 1)
				{
					return true;
				}
				return false;
			}
			return false;
		}
		catch
		{
			return false;
		}
	}

	public float GetProcessorCoreClockFrequency(int _proc_index, int _core_index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(3605638611u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetProcessorCoreClockFrequency)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetProcessorCoreClockFrequency)))(objptr, _proc_index, _core_index);
			}
			return F_UNDEFINED_VALUE;
		}
		catch
		{
			return F_UNDEFINED_VALUE;
		}
	}

	public float GetProcessorCoreClockMultiplier(int _proc_index, int _core_index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(2408783654u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetProcessorCoreClockMultiplier)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetProcessorCoreClockMultiplier)))(objptr, _proc_index, _core_index);
			}
			return F_UNDEFINED_VALUE;
		}
		catch
		{
			return F_UNDEFINED_VALUE;
		}
	}

	public float GetProcessorCoreTemperature(int _proc_index, int _core_index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(1848171601u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetProcessorCoreTemperature)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetProcessorCoreTemperature)))(objptr, _proc_index, _core_index);
			}
			return F_UNDEFINED_VALUE;
		}
		catch
		{
			return F_UNDEFINED_VALUE;
		}
	}

	public float GetBusFrequency()
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(1576844281u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetBusFrequency)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetBusFrequency)))(objptr);
			}
			return F_UNDEFINED_VALUE;
		}
		catch
		{
			return F_UNDEFINED_VALUE;
		}
	}

	public float GetProcessorRatedBusFrequency(int _proc_index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(3416495943u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetProcessorRatedBusFrequency)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetProcessorRatedBusFrequency)))(objptr, _proc_index);
			}
			return F_UNDEFINED_VALUE;
		}
		catch
		{
			return F_UNDEFINED_VALUE;
		}
	}

	public float GetProcessorStockClockFrequency(int _proc_index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(3547965171u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetProcessorStockClockFrequency)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetProcessorStockClockFrequency)))(objptr, _proc_index);
			}
			return F_UNDEFINED_VALUE;
		}
		catch
		{
			return F_UNDEFINED_VALUE;
		}
	}

	public float GetProcessorStockBusFrequency(int _proc_index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(4120505139u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetProcessorStockBusFrequency)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetProcessorStockBusFrequency)))(objptr, _proc_index);
			}
			return F_UNDEFINED_VALUE;
		}
		catch
		{
			return F_UNDEFINED_VALUE;
		}
	}

	public int GetProcessorMaxCacheLevel(int _proc_index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(2156069126u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetProcessorMaxCacheLevel)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetProcessorMaxCacheLevel)))(objptr, _proc_index);
			}
			return 0;
		}
		catch
		{
			return 0;
		}
	}

	public void GetProcessorCacheParameters(int _proc_index, int _core_set, int _cache_level, int _cache_type, ref int _NbCaches, ref int _size)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(1308007405u);
			if (intPtr != IntPtr.Zero)
			{
				((CPUIDSDK_fp_GetProcessorCacheParameters)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetProcessorCacheParameters)))(objptr, _proc_index, _core_set, _cache_level, _cache_type, ref _NbCaches, ref _size);
			}
		}
		catch
		{
		}
	}

	public uint GetProcessorID(int _proc_index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(952529292u);
			if (intPtr != IntPtr.Zero)
			{
				return (uint)((CPUIDSDK_fp_GetProcessorID)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetProcessorID)))(objptr, _proc_index);
			}
			return 0u;
		}
		catch
		{
			return 0u;
		}
	}

	public float GetProcessorVoltageID(int _proc_index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(1195282045u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetProcessorVoltageID)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetProcessorVoltageID)))(objptr, _proc_index);
			}
			return F_UNDEFINED_VALUE;
		}
		catch
		{
			return F_UNDEFINED_VALUE;
		}
	}

	public int GetMemoryType()
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(3674977815u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetMemoryType)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetMemoryType)))(objptr);
			}
			return I_UNDEFINED_VALUE;
		}
		catch
		{
			return I_UNDEFINED_VALUE;
		}
	}

	public int GetMemorySize()
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(3292498047u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetMemorySize)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetMemorySize)))(objptr);
			}
			return I_UNDEFINED_VALUE;
		}
		catch
		{
			return I_UNDEFINED_VALUE;
		}
	}

	public float GetMemoryClockFrequency()
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(2768456198u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetMemoryClockFrequency)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetMemoryClockFrequency)))(objptr);
			}
			return F_UNDEFINED_VALUE;
		}
		catch
		{
			return F_UNDEFINED_VALUE;
		}
	}

	public int GetMemoryNumberOfChannels()
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(2314277858u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetMemoryNumberOfChannels)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetMemoryNumberOfChannels)))(objptr);
			}
			return I_UNDEFINED_VALUE;
		}
		catch
		{
			return I_UNDEFINED_VALUE;
		}
	}

	public float GetMemoryCASLatency()
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(1804916521u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetMemoryCASLatency)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetMemoryCASLatency)))(objptr);
			}
			return F_UNDEFINED_VALUE;
		}
		catch
		{
			return F_UNDEFINED_VALUE;
		}
	}

	public int GetMemoryRAStoCASDelay()
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(2121596137u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetMemoryRAStoCASDelay)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetMemoryRAStoCASDelay)))(objptr);
			}
			return I_UNDEFINED_VALUE;
		}
		catch
		{
			return I_UNDEFINED_VALUE;
		}
	}

	public int GetMemoryRASPrecharge()
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(1957488985u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetMemoryRASPrecharge)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetMemoryRASPrecharge)))(objptr);
			}
			return I_UNDEFINED_VALUE;
		}
		catch
		{
			return I_UNDEFINED_VALUE;
		}
	}

	public int GetMemoryTRAS()
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(2196964838u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetMemoryTRAS)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetMemoryTRAS)))(objptr);
			}
			return I_UNDEFINED_VALUE;
		}
		catch
		{
			return I_UNDEFINED_VALUE;
		}
	}

	public int GetMemoryTRC()
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(690377292u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetMemoryTRC)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetMemoryTRC)))(objptr);
			}
			return I_UNDEFINED_VALUE;
		}
		catch
		{
			return I_UNDEFINED_VALUE;
		}
	}

	public int GetMemoryCommandRate()
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(3405878787u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetMemoryCommandRate)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetMemoryCommandRate)))(objptr);
			}
			return I_UNDEFINED_VALUE;
		}
		catch
		{
			return I_UNDEFINED_VALUE;
		}
	}

	public string GetNorthBridgeVendor()
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(2384272442u);
			if (intPtr != IntPtr.Zero)
			{
				IntPtr ptr = ((CPUIDSDK_fp_GetNorthBridgeVendor)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetNorthBridgeVendor)))(objptr);
				string result = Marshal.PtrToStringAnsi(ptr);
				Marshal.FreeBSTR(ptr);
				return result;
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	public string GetNorthBridgeModel()
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(1063943892u);
			if (intPtr != IntPtr.Zero)
			{
				IntPtr ptr = ((CPUIDSDK_fp_GetNorthBridgeModel)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetNorthBridgeModel)))(objptr);
				string result = Marshal.PtrToStringAnsi(ptr);
				Marshal.FreeBSTR(ptr);
				return result;
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	public string GetNorthBridgeRevision()
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(510278868u);
			if (intPtr != IntPtr.Zero)
			{
				IntPtr ptr = ((CPUIDSDK_fp_GetNorthBridgeRevision)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetNorthBridgeRevision)))(objptr);
				string result = Marshal.PtrToStringAnsi(ptr);
				Marshal.FreeBSTR(ptr);
				return result;
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	public string GetSouthBridgeVendor()
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(3736321383u);
			if (intPtr != IntPtr.Zero)
			{
				IntPtr ptr = ((CPUIDSDK_fp_GetSouthBridgeVendor)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetSouthBridgeVendor)))(objptr);
				string result = Marshal.PtrToStringAnsi(ptr);
				Marshal.FreeBSTR(ptr);
				return result;
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	public string GetSouthBridgeModel()
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(875980908u);
			if (intPtr != IntPtr.Zero)
			{
				IntPtr ptr = ((CPUIDSDK_fp_GetSouthBridgeModel)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetSouthBridgeModel)))(objptr);
				string result = Marshal.PtrToStringAnsi(ptr);
				Marshal.FreeBSTR(ptr);
				return result;
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	public string GetSouthBridgeRevision()
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(1052802432u);
			if (intPtr != IntPtr.Zero)
			{
				IntPtr ptr = ((CPUIDSDK_fp_GetSouthBridgeRevision)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetSouthBridgeRevision)))(objptr);
				string result = Marshal.PtrToStringAnsi(ptr);
				Marshal.FreeBSTR(ptr);
				return result;
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	public void GetMemorySlotsConfig(ref int _nbslots, ref int _nbusedslots, ref int _slotmap_h, ref int _slotmap_l, ref int _maxmodulesize)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(4214748783u);
			if (intPtr != IntPtr.Zero)
			{
				((CPUIDSDK_fp_GetMemorySlotsConfig)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetMemorySlotsConfig)))(objptr, ref _nbslots, ref _nbusedslots, ref _slotmap_h, ref _slotmap_l, ref _maxmodulesize);
			}
		}
		catch
		{
		}
	}

	public string GetBIOSVendor()
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(3665147115u);
			if (intPtr != IntPtr.Zero)
			{
				IntPtr ptr = ((CPUIDSDK_fp_GetBIOSVendor)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetBIOSVendor)))(objptr);
				string result = Marshal.PtrToStringAnsi(ptr);
				Marshal.FreeBSTR(ptr);
				return result;
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	public string GetBIOSVersion()
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(3065212262u);
			if (intPtr != IntPtr.Zero)
			{
				IntPtr ptr = ((CPUIDSDK_fp_GetBIOSVersion)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetBIOSVersion)))(objptr);
				string result = Marshal.PtrToStringAnsi(ptr);
				Marshal.FreeBSTR(ptr);
				return result;
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	public string GetBIOSDate()
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(3581782779u);
			if (intPtr != IntPtr.Zero)
			{
				IntPtr ptr = ((CPUIDSDK_fp_GetBIOSDate)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetBIOSDate)))(objptr);
				string result = Marshal.PtrToStringAnsi(ptr);
				Marshal.FreeBSTR(ptr);
				return result;
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	public string GetMainboardVendor()
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(1593884161u);
			if (intPtr != IntPtr.Zero)
			{
				IntPtr ptr = ((CPUIDSDK_fp_GetMainboardVendor)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetMainboardVendor)))(objptr);
				string result = Marshal.PtrToStringAnsi(ptr);
				Marshal.FreeBSTR(ptr);
				return result;
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	public string GetMainboardModel()
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(1055948256u);
			if (intPtr != IntPtr.Zero)
			{
				IntPtr ptr = ((CPUIDSDK_fp_GetMainboardModel)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetMainboardModel)))(objptr);
				string result = Marshal.PtrToStringAnsi(ptr);
				Marshal.FreeBSTR(ptr);
				return result;
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	public string GetMainboardRevision()
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(345516336u);
			if (intPtr != IntPtr.Zero)
			{
				IntPtr ptr = ((CPUIDSDK_fp_GetMainboardRevision)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetMainboardRevision)))(objptr);
				string result = Marshal.PtrToStringAnsi(ptr);
				Marshal.FreeBSTR(ptr);
				return result;
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	public string GetMainboardSerialNumber()
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(1068138324u);
			if (intPtr != IntPtr.Zero)
			{
				IntPtr ptr = ((CPUIDSDK_fp_GetMainboardSerialNumber)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetMainboardSerialNumber)))(objptr);
				string result = Marshal.PtrToStringAnsi(ptr);
				Marshal.FreeBSTR(ptr);
				return result;
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	public string GetSystemManufacturer()
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(227547936u);
			if (intPtr != IntPtr.Zero)
			{
				IntPtr ptr = ((CPUIDSDK_fp_GetSystemManufacturer)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetSystemManufacturer)))(objptr);
				string result = Marshal.PtrToStringAnsi(ptr);
				Marshal.FreeBSTR(ptr);
				return result;
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	public string GetSystemProductName()
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(3833973003u);
			if (intPtr != IntPtr.Zero)
			{
				IntPtr ptr = ((CPUIDSDK_fp_GetSystemProductName)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetSystemProductName)))(objptr);
				string result = Marshal.PtrToStringAnsi(ptr);
				Marshal.FreeBSTR(ptr);
				return result;
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	public string GetSystemVersion()
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(1357029829u);
			if (intPtr != IntPtr.Zero)
			{
				IntPtr ptr = ((CPUIDSDK_fp_GetSystemVersion)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetSystemVersion)))(objptr);
				string result = Marshal.PtrToStringAnsi(ptr);
				Marshal.FreeBSTR(ptr);
				return result;
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	public string GetSystemSerialNumber()
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(1306041265u);
			if (intPtr != IntPtr.Zero)
			{
				IntPtr ptr = ((CPUIDSDK_fp_GetSystemSerialNumber)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetSystemSerialNumber)))(objptr);
				string result = Marshal.PtrToStringAnsi(ptr);
				Marshal.FreeBSTR(ptr);
				return result;
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	public string GetSystemUUID()
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(3847867059u);
			if (intPtr != IntPtr.Zero)
			{
				IntPtr ptr = ((CPUIDSDK_fp_GetSystemUUID)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetSystemUUID)))(objptr);
				string result = Marshal.PtrToStringAnsi(ptr);
				Marshal.FreeBSTR(ptr);
				return result;
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	public string GetChassisManufacturer()
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(1678559257u);
			if (intPtr != IntPtr.Zero)
			{
				IntPtr ptr = ((CPUIDSDK_fp_GetChassisManufacturer)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetChassisManufacturer)))(objptr);
				string result = Marshal.PtrToStringAnsi(ptr);
				Marshal.FreeBSTR(ptr);
				return result;
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	public string GetChassisType()
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(3882995427u);
			if (intPtr != IntPtr.Zero)
			{
				IntPtr ptr = ((CPUIDSDK_fp_GetChassisType)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetChassisType)))(objptr);
				string result = Marshal.PtrToStringAnsi(ptr);
				Marshal.FreeBSTR(ptr);
				return result;
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	public string GetChassisSerialNumber()
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(2833207742u);
			if (intPtr != IntPtr.Zero)
			{
				IntPtr ptr = ((CPUIDSDK_fp_GetChassisSerialNumber)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetChassisSerialNumber)))(objptr);
				string result = Marshal.PtrToStringAnsi(ptr);
				Marshal.FreeBSTR(ptr);
				return result;
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	public bool GetMemoryInfosExt(ref string _szLocation, ref string _szUsage, ref string _szCorrection)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(1091207701u);
			if (intPtr != IntPtr.Zero)
			{
				IntPtr _szLocation2 = IntPtr.Zero;
				IntPtr _szUsage2 = IntPtr.Zero;
				IntPtr _szCorrection2 = IntPtr.Zero;
				if (((CPUIDSDK_fp_GetMemoryInfosExt)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetMemoryInfosExt)))(objptr, ref _szLocation2, ref _szUsage2, ref _szCorrection2) == 1)
				{
					_szLocation = Marshal.PtrToStringAnsi(_szLocation2);
					Marshal.FreeBSTR(_szLocation2);
					_szUsage = Marshal.PtrToStringAnsi(_szUsage2);
					Marshal.FreeBSTR(_szUsage2);
					_szCorrection = Marshal.PtrToStringAnsi(_szCorrection2);
					Marshal.FreeBSTR(_szCorrection2);
					return true;
				}
				return false;
			}
			return false;
		}
		catch
		{
			return false;
		}
	}

	public int GetNumberOfMemoryDevices()
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(2376670034u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetNumberOfMemoryDevices)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetNumberOfMemoryDevices)))(objptr);
			}
			return I_UNDEFINED_VALUE;
		}
		catch
		{
			return I_UNDEFINED_VALUE;
		}
	}

	public bool GetMemoryDeviceInfos(int _device_index, ref int _size, ref string _szFormat)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(1665582733u);
			if (intPtr != IntPtr.Zero)
			{
				IntPtr _szFormat2 = IntPtr.Zero;
				if (((CPUIDSDK_fp_GetMemoryDeviceInfos)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetMemoryDeviceInfos)))(objptr, _device_index, ref _size, ref _szFormat2) == 1)
				{
					_szFormat = Marshal.PtrToStringAnsi(_szFormat2);
					Marshal.FreeBSTR(_szFormat2);
					return true;
				}
				return false;
			}
			return false;
		}
		catch
		{
			return false;
		}
	}

	public bool GetMemoryDeviceInfosExt(int _device_index, ref string _szDesignation, ref string _szType, ref int _total_width, ref int _data_width, ref int _speed)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(2646031214u);
			if (intPtr != IntPtr.Zero)
			{
				IntPtr _szDesignation2 = IntPtr.Zero;
				IntPtr _szType2 = IntPtr.Zero;
				if (((CPUIDSDK_fp_GetMemoryDeviceInfosExt)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetMemoryDeviceInfosExt)))(objptr, _device_index, ref _szDesignation2, ref _szType2, ref _total_width, ref _data_width, ref _speed) == 1)
				{
					_szDesignation = Marshal.PtrToStringAnsi(_szDesignation2);
					Marshal.FreeBSTR(_szDesignation2);
					_szType = Marshal.PtrToStringAnsi(_szType2);
					Marshal.FreeBSTR(_szType2);
					return true;
				}
				return false;
			}
			return false;
		}
		catch
		{
			return false;
		}
	}

	public int GetMemoryMaxCapacity()
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(1083212065u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetMemoryMaxCapacity)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetMemoryMaxCapacity)))(objptr);
			}
			return I_UNDEFINED_VALUE;
		}
		catch
		{
			return I_UNDEFINED_VALUE;
		}
	}

	public int GetMemoryMaxNumberOfDevices()
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(2376538958u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetMemoryMaxNumberOfDevices)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetMemoryMaxNumberOfDevices)))(objptr);
			}
			return I_UNDEFINED_VALUE;
		}
		catch
		{
			return I_UNDEFINED_VALUE;
		}
	}

	public int GetProcessorSockets()
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(2271022778u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetProcessorSockets)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetProcessorSockets)))(objptr);
			}
			return I_UNDEFINED_VALUE;
		}
		catch
		{
			return I_UNDEFINED_VALUE;
		}
	}

	public string GetSystemSKU()
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(702829512u);
			if (intPtr != IntPtr.Zero)
			{
				IntPtr ptr = ((CPUIDSDK_fp_GetSystemSKU)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetSystemSKU)))(objptr);
				string result = Marshal.PtrToStringAnsi(ptr);
				Marshal.FreeBSTR(ptr);
				return result;
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	public string GetSystemFamily()
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(227679012u);
			if (intPtr != IntPtr.Zero)
			{
				IntPtr ptr = ((CPUIDSDK_fp_GetSystemFamily)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetSystemFamily)))(objptr);
				string result = Marshal.PtrToStringAnsi(ptr);
				Marshal.FreeBSTR(ptr);
				return result;
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	public int GetNumberOfSPDModules()
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(2647079822u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetNumberOfSPDModules)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetNumberOfSPDModules)))(objptr);
			}
			return I_UNDEFINED_VALUE;
		}
		catch
		{
			return I_UNDEFINED_VALUE;
		}
	}

	public int GetSPDModuleType(int _spd_index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(156635820u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetSPDModuleType)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetSPDModuleType)))(objptr, _spd_index);
			}
			return I_UNDEFINED_VALUE;
		}
		catch
		{
			return I_UNDEFINED_VALUE;
		}
	}

	public int GetSPDModuleSize(int _spd_index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(2193425786u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetSPDModuleSize)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetSPDModuleSize)))(objptr, _spd_index);
			}
			return I_UNDEFINED_VALUE;
		}
		catch
		{
			return I_UNDEFINED_VALUE;
		}
	}

	public string GetSPDModuleFormat(int _spd_index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(757619280u);
			if (intPtr != IntPtr.Zero)
			{
				IntPtr ptr = ((CPUIDSDK_fp_GetSPDModuleFormat)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetSPDModuleFormat)))(objptr, _spd_index);
				string result = Marshal.PtrToStringAnsi(ptr);
				Marshal.FreeBSTR(ptr);
				return result;
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	public string GetSPDModuleManufacturer(int _spd_index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(1777259485u);
			if (intPtr != IntPtr.Zero)
			{
				IntPtr ptr = ((CPUIDSDK_fp_GetSPDModuleManufacturer)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetSPDModuleManufacturer)))(objptr, _spd_index);
				string result = Marshal.PtrToStringAnsi(ptr);
				Marshal.FreeBSTR(ptr);
				return result;
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	public bool GetSPDModuleManufacturerID(int _spd_index, byte[] _id)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(4025343963u);
			if (intPtr != IntPtr.Zero)
			{
				if (((CPUIDSDK_fp_GetSPDModuleManufacturerID)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetSPDModuleManufacturerID)))(objptr, _spd_index, _id) == 1)
				{
					return true;
				}
				return false;
			}
			return false;
		}
		catch
		{
			return false;
		}
	}

	public string GetSPDModuleDRAMManufacturer(int _spd_index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(3000198566u);
			if (intPtr != IntPtr.Zero)
			{
				IntPtr ptr = ((CPUIDSDK_fp_GetSPDModuleDRAMManufacturer)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetSPDModuleDRAMManufacturer)))(objptr, _spd_index);
				string result = Marshal.PtrToStringAnsi(ptr);
				Marshal.FreeBSTR(ptr);
				return result;
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	public int GetSPDModuleMaxFrequency(int _spd_index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(980186328u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetSPDModuleMaxFrequency)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetSPDModuleMaxFrequency)))(objptr, _spd_index);
			}
			return I_UNDEFINED_VALUE;
		}
		catch
		{
			return I_UNDEFINED_VALUE;
		}
	}

	public string GetSPDModuleSpecification(int _spd_index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(3248849739u);
			if (intPtr != IntPtr.Zero)
			{
				IntPtr ptr = ((CPUIDSDK_fp_GetSPDModuleSpecification)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetSPDModuleSpecification)))(objptr, _spd_index);
				string result = Marshal.PtrToStringAnsi(ptr);
				Marshal.FreeBSTR(ptr);
				return result;
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	public string GetSPDModulePartNumber(int _spd_index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(57018060u);
			if (intPtr != IntPtr.Zero)
			{
				IntPtr ptr = ((CPUIDSDK_fp_GetSPDModulePartNumber)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetSPDModulePartNumber)))(objptr, _spd_index);
				string result = Marshal.PtrToStringAnsi(ptr);
				Marshal.FreeBSTR(ptr);
				return result;
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	public string GetSPDModuleSerialNumber(int _spd_index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(560480976u);
			if (intPtr != IntPtr.Zero)
			{
				IntPtr ptr = ((CPUIDSDK_fp_GetSPDModuleSerialNumber)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetSPDModuleSerialNumber)))(objptr, _spd_index);
				string result = Marshal.PtrToStringAnsi(ptr);
				Marshal.FreeBSTR(ptr);
				return result;
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	public float GetSPDModuleMinTRCD(int _spd_index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(4129811535u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetSPDModuleMinTRCD)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetSPDModuleMinTRCD)))(objptr, _spd_index);
			}
			return F_UNDEFINED_VALUE;
		}
		catch
		{
			return F_UNDEFINED_VALUE;
		}
	}

	public float GetSPDModuleMinTRP(int _spd_index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(2896779602u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetSPDModuleMinTRP)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetSPDModuleMinTRP)))(objptr, _spd_index);
			}
			return F_UNDEFINED_VALUE;
		}
		catch
		{
			return F_UNDEFINED_VALUE;
		}
	}

	public float GetSPDModuleMinTRAS(int _spd_index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(1368826669u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetSPDModuleMinTRAS)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetSPDModuleMinTRAS)))(objptr, _spd_index);
			}
			return F_UNDEFINED_VALUE;
		}
		catch
		{
			return F_UNDEFINED_VALUE;
		}
	}

	public float GetSPDModuleMinTRC(int _spd_index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(2666347994u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetSPDModuleMinTRC)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetSPDModuleMinTRC)))(objptr, _spd_index);
			}
			return F_UNDEFINED_VALUE;
		}
		catch
		{
			return F_UNDEFINED_VALUE;
		}
	}

	public int GetSPDModuleManufacturingDate(int _spd_index, ref int _year, ref int _week)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(1955391769u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetSPDModuleManufacturingDate)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetSPDModuleManufacturingDate)))(objptr, _spd_index, ref _year, ref _week);
			}
			return I_UNDEFINED_VALUE;
		}
		catch
		{
			return I_UNDEFINED_VALUE;
		}
	}

	public int GetSPDModuleNumberOfBanks(int _spd_index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(210376980u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetSPDModuleNumberOfBanks)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetSPDModuleNumberOfBanks)))(objptr, _spd_index);
			}
			return I_UNDEFINED_VALUE;
		}
		catch
		{
			return I_UNDEFINED_VALUE;
		}
	}

	public int GetSPDModuleDataWidth(int _spd_index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(349710768u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetSPDModuleDataWidth)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetSPDModuleDataWidth)))(objptr, _spd_index);
			}
			return I_UNDEFINED_VALUE;
		}
		catch
		{
			return I_UNDEFINED_VALUE;
		}
	}

	public float GetSPDModuleTemperature(int _spd_index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(749361492u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetSPDModuleTemperature)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetSPDModuleTemperature)))(objptr, _spd_index);
			}
			return F_UNDEFINED_VALUE;
		}
		catch
		{
			return F_UNDEFINED_VALUE;
		}
	}

	public int GetSPDModuleNumberOfProfiles(int _spd_index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(4029145167u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetSPDModuleNumberOfProfiles)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetSPDModuleNumberOfProfiles)))(objptr, _spd_index);
			}
			return I_UNDEFINED_VALUE;
		}
		catch
		{
			return I_UNDEFINED_VALUE;
		}
	}

	public void GetSPDModuleProfileInfos(int _spd_index, int _profile_index, ref float _frequency, ref float _tCL, ref float _nominal_vdd)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(3121837094u);
			if (intPtr != IntPtr.Zero)
			{
				((CPUIDSDK_fp_GetSPDModuleProfileInfos)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetSPDModuleProfileInfos)))(objptr, _spd_index, _profile_index, ref _frequency, ref _tCL, ref _nominal_vdd);
			}
		}
		catch
		{
		}
	}

	public int GetSPDModuleNumberOfEPPProfiles(int _spd_index, ref int _epp_revision)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(4004896107u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetSPDModuleNumberOfEPPProfiles)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetSPDModuleNumberOfEPPProfiles)))(objptr, _spd_index, ref _epp_revision);
			}
			return I_UNDEFINED_VALUE;
		}
		catch
		{
			return I_UNDEFINED_VALUE;
		}
	}

	public void GetSPDModuleEPPProfileInfos(int _spd_index, int _profile_index, ref float _frequency, ref float _tCL, ref float _tRCD, ref float _tRAS, ref float _tRP, ref float _tRC, ref float _nominal_vdd)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(1600700113u);
			if (intPtr != IntPtr.Zero)
			{
				((CPUIDSDK_fp_GetSPDModuleEPPProfileInfos)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetSPDModuleEPPProfileInfos)))(objptr, _spd_index, _profile_index, ref _frequency, ref _tCL, ref _tRCD, ref _tRAS, ref _tRP, ref _tRC, ref _nominal_vdd);
			}
		}
		catch
		{
		}
	}

	public int GetSPDModuleNumberOfXMPProfiles(int _spd_index, ref int _xmp_revision)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(33948684u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetSPDModuleNumberOfXMPProfiles)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetSPDModuleNumberOfXMPProfiles)))(objptr, _spd_index, ref _xmp_revision);
			}
			return I_UNDEFINED_VALUE;
		}
		catch
		{
			return I_UNDEFINED_VALUE;
		}
	}

	public int GetSPDModuleXMPProfileNumberOfCL(int _spd_index, int _profile_index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(3356463135u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetSPDModuleXMPProfileNumberOfCL)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetSPDModuleXMPProfileNumberOfCL)))(objptr, _spd_index, _profile_index);
			}
			return I_UNDEFINED_VALUE;
		}
		catch
		{
			return I_UNDEFINED_VALUE;
		}
	}

	public void GetSPDModuleXMPProfileCLInfos(int _spd_index, int _profile_index, int _cl_index, ref float _frequency, ref float _CL)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(3955611531u);
			if (intPtr != IntPtr.Zero)
			{
				((CPUIDSDK_fp_GetSPDModuleXMPProfileCLInfos)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetSPDModuleXMPProfileCLInfos)))(objptr, _spd_index, _profile_index, _cl_index, ref _frequency, ref _CL);
			}
		}
		catch
		{
		}
	}

	public void GetSPDModuleXMPProfileInfos(int _spd_index, int _profile_index, ref float _tRCD, ref float _tRAS, ref float _tRP, ref float _tRC, ref float _nominal_vdd, ref int _max_freq, ref float _max_CL)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(3446774499u);
			if (intPtr != IntPtr.Zero)
			{
				((CPUIDSDK_fp_GetSPDModuleXMPProfileInfos)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetSPDModuleXMPProfileInfos)))(objptr, _spd_index, _profile_index, ref _tRCD, ref _tRAS, ref _tRP, ref _tRC, ref _nominal_vdd, ref _max_freq, ref _max_CL);
			}
		}
		catch
		{
		}
	}

	public int GetSPDModuleNumberOfAMPProfiles(int _spd_index, ref int _amp_revision)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(3830958255u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetSPDModuleNumberOfAMPProfiles)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetSPDModuleNumberOfAMPProfiles)))(objptr, _spd_index, ref _amp_revision);
			}
			return I_UNDEFINED_VALUE;
		}
		catch
		{
			return I_UNDEFINED_VALUE;
		}
	}

	public void GetSPDModuleAMPProfileInfos(int _spd_index, int _profile_index, ref int _frequency, ref float _min_cycle_time, ref float _tCL, ref float _tRCD, ref float _tRAS, ref float _tRP, ref float _tRC)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(2638559882u);
			if (intPtr != IntPtr.Zero)
			{
				((CPUIDSDK_fp_GetSPDModuleAMPProfileInfos)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetSPDModuleAMPProfileInfos)))(objptr, _spd_index, _profile_index, ref _frequency, ref _min_cycle_time, ref _tCL, ref _tRCD, ref _tRAS, ref _tRP, ref _tRC);
			}
		}
		catch
		{
		}
	}

	public int GetSPDModuleRawData(int _spd_index, int _offset)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(628902648u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetSPDModuleRawData)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetSPDModuleRawData)))(objptr, _spd_index, _offset);
			}
			return I_UNDEFINED_VALUE;
		}
		catch
		{
			return I_UNDEFINED_VALUE;
		}
	}

	public int GetNumberOfDisplayAdapter()
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(946368720u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetNumberOfDisplayAdapter)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetNumberOfDisplayAdapter)))(objptr);
			}
			return I_UNDEFINED_VALUE;
		}
		catch
		{
			return I_UNDEFINED_VALUE;
		}
	}

	public string GetDisplayAdapterName(int _adapter_index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(3414529803u);
			if (intPtr != IntPtr.Zero)
			{
				IntPtr ptr = ((CPUIDSDK_fp_GetDisplayAdapterName)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetDisplayAdapterName)))(objptr, _adapter_index);
				string result = Marshal.PtrToStringAnsi(ptr);
				Marshal.FreeBSTR(ptr);
				return result;
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	public string GetDisplayAdapterCodeName(int _adapter_index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(3373371939u);
			if (intPtr != IntPtr.Zero)
			{
				IntPtr ptr = ((CPUIDSDK_fp_GetDisplayAdapterCodeName)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetDisplayAdapterCodeName)))(objptr, _adapter_index);
				string result = Marshal.PtrToStringAnsi(ptr);
				Marshal.FreeBSTR(ptr);
				return result;
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	public int GetDisplayAdapterNumberOfPerformanceLevels(int _adapter_index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(3804480903u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetDisplayAdapterNumberOfPerformanceLevels)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetDisplayAdapterNumberOfPerformanceLevels)))(objptr, _adapter_index);
			}
			return I_UNDEFINED_VALUE;
		}
		catch
		{
			return I_UNDEFINED_VALUE;
		}
	}

	public int GetDisplayAdapterCurrentPerformanceLevel(int _adapter_index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(2263027142u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetDisplayAdapterCurrentPerformanceLevel)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetDisplayAdapterCurrentPerformanceLevel)))(objptr, _adapter_index);
			}
			return I_UNDEFINED_VALUE;
		}
		catch
		{
			return I_UNDEFINED_VALUE;
		}
	}

	public string GetDisplayAdapterPerformanceLevelName(int _adapter_index, int _perf_level)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(93195036u);
			if (intPtr != IntPtr.Zero)
			{
				IntPtr ptr = ((CPUIDSDK_fp_GetDisplayAdapterPerformanceLevelName)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetDisplayAdapterPerformanceLevelName)))(objptr, _adapter_index, _perf_level);
				string result = Marshal.PtrToStringAnsi(ptr);
				Marshal.FreeBSTR(ptr);
				return result;
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	public float GetDisplayAdapterClock(int _adapter_index, int _perf_level, int _domain)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(1968368293u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetDisplayAdapterClock)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetDisplayAdapterClock)))(objptr, _adapter_index, _perf_level, _domain);
			}
			return F_UNDEFINED_VALUE;
		}
		catch
		{
			return F_UNDEFINED_VALUE;
		}
	}

	public float GetDisplayAdapterStockClock(int _adapter_index, int _perf_level, int _domain)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(724719204u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetDisplayAdapterStockClock)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetDisplayAdapterStockClock)))(objptr, _adapter_index, _perf_level, _domain);
			}
			return F_UNDEFINED_VALUE;
		}
		catch
		{
			return F_UNDEFINED_VALUE;
		}
	}

	public float GetDisplayAdapterManufacturingProcess(int _adapter_index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(4095993927u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetDisplayAdapterManufacturingProcess)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetDisplayAdapterManufacturingProcess)))(objptr, _adapter_index);
			}
			return F_UNDEFINED_VALUE;
		}
		catch
		{
			return F_UNDEFINED_VALUE;
		}
	}

	public float GetDisplayAdapterTemperature(int _adapter_index, int _domain)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(3432618291u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetDisplayAdapterTemperature)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetDisplayAdapterTemperature)))(objptr, _adapter_index, _domain);
			}
			return F_UNDEFINED_VALUE;
		}
		catch
		{
			return F_UNDEFINED_VALUE;
		}
	}

	public int GetDisplayAdapterFanSpeed(int _adapter_index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(2485594190u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetDisplayAdapterFanSpeed)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetDisplayAdapterFanSpeed)))(objptr, _adapter_index);
			}
			return I_UNDEFINED_VALUE;
		}
		catch
		{
			return I_UNDEFINED_VALUE;
		}
	}

	public int GetDisplayAdapterFanPWM(int _adapter_index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(1773589357u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetDisplayAdapterFanPWM)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetDisplayAdapterFanPWM)))(objptr, _adapter_index);
			}
			return I_UNDEFINED_VALUE;
		}
		catch
		{
			return I_UNDEFINED_VALUE;
		}
	}

	public bool GetDisplayAdapterMemoryType(int _adapter_index, ref int _type)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(1830214189u);
			if (intPtr != IntPtr.Zero)
			{
				if (((CPUIDSDK_fp_GetDisplayAdapterMemoryType)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetDisplayAdapterMemoryType)))(objptr, _adapter_index, ref _type) == 1)
				{
					return true;
				}
				return false;
			}
			return false;
		}
		catch
		{
			return false;
		}
	}

	public bool GetDisplayAdapterMemorySize(int _adapter_index, ref int _size)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(1983835261u);
			if (intPtr != IntPtr.Zero)
			{
				if (((CPUIDSDK_fp_GetDisplayAdapterMemorySize)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetDisplayAdapterMemorySize)))(objptr, _adapter_index, ref _size) == 1)
				{
					return true;
				}
				return false;
			}
			return false;
		}
		catch
		{
			return false;
		}
	}

	public bool GetDisplayAdapterMemoryBusWidth(int _adapter_index, ref int _bus_width)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(1721158957u);
			if (intPtr != IntPtr.Zero)
			{
				if (((CPUIDSDK_fp_GetDisplayAdapterMemoryBusWidth)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetDisplayAdapterMemoryBusWidth)))(objptr, _adapter_index, ref _bus_width) == 1)
				{
					return true;
				}
				return false;
			}
			return false;
		}
		catch
		{
			return false;
		}
	}

	public string GetDisplayAdapterMemoryVendor(int _adapter_index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(1704119077u);
			if (intPtr != IntPtr.Zero)
			{
				IntPtr ptr = ((CPUIDSDK_fp_GetDisplayAdapterMemoryVendor)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetDisplayAdapterMemoryVendor)))(objptr, _adapter_index);
				string result = Marshal.PtrToStringAnsi(ptr);
				Marshal.FreeBSTR(ptr);
				return result;
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	public string GetDisplayAdaterDriverVersion(int _adapter_index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(3181345598u);
			if (intPtr != IntPtr.Zero)
			{
				IntPtr ptr = ((CPUIDSDK_fp_GetDisplayAdapterDriverVersion)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetDisplayAdapterDriverVersion)))(objptr, _adapter_index);
				string result = Marshal.PtrToStringAnsi(ptr);
				Marshal.FreeBSTR(ptr);
				return result;
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	public string GetDirectXVersion()
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(1454943601u);
			if (intPtr != IntPtr.Zero)
			{
				IntPtr ptr = ((CPUIDSDK_fp_GetDirectXVersion)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetDirectXVersion)))(objptr);
				string result = Marshal.PtrToStringAnsi(ptr);
				Marshal.FreeBSTR(ptr);
				return result;
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	public bool GetDisplayAdapterBusInfos(int _adapter_index, ref int _bus_type, ref int _multi_vpu)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(2428182902u);
			if (intPtr != IntPtr.Zero)
			{
				if (((CPUIDSDK_fp_GetDisplayAdapterBusInfos)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetDisplayAdapterBusInfos)))(objptr, _adapter_index, ref _bus_type, ref _multi_vpu) == 1)
				{
					return true;
				}
				return false;
			}
			return false;
		}
		catch
		{
			return false;
		}
	}

	public string GetDisplayAdapterCoreFamily(int _adapter_index, ref int _core)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(2643933998u);
			if (intPtr != IntPtr.Zero)
			{
				IntPtr ptr = ((CPUIDSDK_fp_GetDisplayAdapterCoreFamily)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetDisplayAdapterCoreFamily)))(objptr, _adapter_index, ref _core);
				string result = Marshal.PtrToStringAnsi(ptr);
				Marshal.FreeBSTR(ptr);
				return result;
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	public bool GetDisplayAdapterPCIAddress(int _adapter_index, ref int _bus, ref int _device, ref int _function)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(2575774478u);
			if (intPtr != IntPtr.Zero)
			{
				if (((CPUIDSDK_fp_GetDisplayAdapterPCIAddress)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetDisplayAdapterPCIAddress)))(objptr, _adapter_index, ref _bus, ref _device, ref _function) == 1)
				{
					return true;
				}
				return false;
			}
			return false;
		}
		catch
		{
			return false;
		}
	}

	public int GetNumberOfMonitors()
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(2989188182u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetNumberOfMonitors)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetNumberOfMonitors)))(objptr);
			}
			return I_UNDEFINED_VALUE;
		}
		catch
		{
			return I_UNDEFINED_VALUE;
		}
	}

	public string GetMonitorName(int _monitor_index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(669405132u);
			if (intPtr != IntPtr.Zero)
			{
				IntPtr ptr = ((CPUIDSDK_fp_GetMonitorName)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetMonitorName)))(objptr, _monitor_index);
				string result = Marshal.PtrToStringAnsi(ptr);
				Marshal.FreeBSTR(ptr);
				return result;
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	public string GetMonitorVendor(int _monitor_index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(390344328u);
			if (intPtr != IntPtr.Zero)
			{
				IntPtr ptr = ((CPUIDSDK_fp_GetMonitorVendor)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetMonitorVendor)))(objptr, _monitor_index);
				string result = Marshal.PtrToStringAnsi(ptr);
				Marshal.FreeBSTR(ptr);
				return result;
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	public string GetMonitorID(int _monitor_index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(345123108u);
			if (intPtr != IntPtr.Zero)
			{
				IntPtr ptr = ((CPUIDSDK_fp_GetMonitorID)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetMonitorID)))(objptr, _monitor_index);
				string result = Marshal.PtrToStringAnsi(ptr);
				Marshal.FreeBSTR(ptr);
				return result;
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	public string GetMonitorSerial(int _monitor_index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(1409722381u);
			if (intPtr != IntPtr.Zero)
			{
				IntPtr ptr = ((CPUIDSDK_fp_GetMonitorSerial)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetMonitorSerial)))(objptr, _monitor_index);
				string result = Marshal.PtrToStringAnsi(ptr);
				Marshal.FreeBSTR(ptr);
				return result;
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	public bool GetMonitorManufacturingDate(int _monitor_index, ref int _week, ref int _year)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(1406052253u);
			if (intPtr != IntPtr.Zero)
			{
				if (((CPUIDSDK_fp_GetMonitorManufacturingDate)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetMonitorManufacturingDate)))(objptr, _monitor_index, ref _week, ref _year) == 1)
				{
					return true;
				}
				return false;
			}
			return false;
		}
		catch
		{
			return false;
		}
	}

	public float GetMonitorSize(int _monitor_index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(2615883734u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetMonitorSize)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetMonitorSize)))(objptr, _monitor_index);
			}
			return F_UNDEFINED_VALUE;
		}
		catch
		{
			return F_UNDEFINED_VALUE;
		}
	}

	public bool GetMonitorResolution(int _monitor_index, ref int _width, ref int _height, ref int _frequency)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(726423192u);
			if (intPtr != IntPtr.Zero)
			{
				if (((CPUIDSDK_fp_GetMonitorResolution)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetMonitorResolution)))(objptr, _monitor_index, ref _width, ref _height, ref _frequency) == 1)
				{
					return true;
				}
				return false;
			}
			return false;
		}
		catch
		{
			return false;
		}
	}

	public int GetMonitorMaxPixelClock(int _monitor_index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(1784337589u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetMonitorMaxPixelClock)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetMonitorMaxPixelClock)))(objptr, _monitor_index);
			}
			return I_UNDEFINED_VALUE;
		}
		catch
		{
			return I_UNDEFINED_VALUE;
		}
	}

	public float GetMonitorGamma(int _monitor_index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(2851558382u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetMonitorGamma)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetMonitorGamma)))(objptr, _monitor_index);
			}
			return F_UNDEFINED_VALUE;
		}
		catch
		{
			return F_UNDEFINED_VALUE;
		}
	}

	public int GetNumberOfStorageDevice()
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(1577237509u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetNumberOfStorageDevice)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetNumberOfStorageDevice)))(objptr);
			}
			return I_UNDEFINED_VALUE;
		}
		catch
		{
			return I_UNDEFINED_VALUE;
		}
	}

	public int GetStorageDriveNumber(int _index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(3523060731u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetStorageDriveNumber)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetStorageDriveNumber)))(objptr, _index);
			}
			return I_UNDEFINED_VALUE;
		}
		catch
		{
			return I_UNDEFINED_VALUE;
		}
	}

	public string GetStorageDeviceName(int _index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(1196592805u);
			if (intPtr != IntPtr.Zero)
			{
				IntPtr ptr = ((CPUIDSDK_fp_GetStorageDeviceName)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetStorageDeviceName)))(objptr, _index);
				string result = Marshal.PtrToStringAnsi(ptr);
				Marshal.FreeBSTR(ptr);
				return result;
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	public string GetStorageDeviceRevision(int _index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(1754059033u);
			if (intPtr != IntPtr.Zero)
			{
				IntPtr ptr = ((CPUIDSDK_fp_GetStorageDeviceRevision)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetStorageDeviceRevision)))(objptr, _index);
				string result = Marshal.PtrToStringAnsi(ptr);
				Marshal.FreeBSTR(ptr);
				return result;
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	public string GetStorageDeviceSerialNumber(int _index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(2153709758u);
			if (intPtr != IntPtr.Zero)
			{
				IntPtr ptr = ((CPUIDSDK_fp_GetStorageDeviceSerialNumber)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetStorageDeviceSerialNumber)))(objptr, _index);
				string result = Marshal.PtrToStringAnsi(ptr);
				Marshal.FreeBSTR(ptr);
				return result;
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	public int GetStorageDeviceBusType(int _index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(3820472175u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetStorageDeviceBusType)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetStorageDeviceBusType)))(objptr, _index);
			}
			return I_UNDEFINED_VALUE;
		}
		catch
		{
			return I_UNDEFINED_VALUE;
		}
	}

	public int GetStorageDeviceRotationSpeed(int _index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(2165506598u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetStorageDeviceRotationSpeed)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetStorageDeviceRotationSpeed)))(objptr, _index);
			}
			return I_UNDEFINED_VALUE;
		}
		catch
		{
			return I_UNDEFINED_VALUE;
		}
	}

	public uint GetStorageDeviceFeatureFlag(int _index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(4152618759u);
			if (intPtr != IntPtr.Zero)
			{
				return (uint)((CPUIDSDK_fp_GetStorageDeviceFeatureFlag)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetStorageDeviceFeatureFlag)))(objptr, _index);
			}
			return (uint)I_UNDEFINED_VALUE;
		}
		catch
		{
			return (uint)I_UNDEFINED_VALUE;
		}
	}

	public int GetStorageDeviceNumberOfVolumes(int _index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(3667637559u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetStorageDeviceNumberOfVolumes)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetStorageDeviceNumberOfVolumes)))(objptr, _index);
			}
			return I_UNDEFINED_VALUE;
		}
		catch
		{
			return I_UNDEFINED_VALUE;
		}
	}

	public string GetStorageDeviceVolumeLetter(int _index, int _volume_index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(534134700u);
			if (intPtr != IntPtr.Zero)
			{
				IntPtr ptr = ((CPUIDSDK_fp_GetStorageDeviceVolumeLetter)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetStorageDeviceVolumeLetter)))(objptr, _index, _volume_index);
				string result = Marshal.PtrToStringAnsi(ptr);
				Marshal.FreeBSTR(ptr);
				return result;
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	public float GetStorageDeviceVolumeTotalCapacity(int _index, int _volume_index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(3118822346u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetStorageDeviceVolumeTotalCapacity)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetStorageDeviceVolumeTotalCapacity)))(objptr, _index, _volume_index);
			}
			return F_UNDEFINED_VALUE;
		}
		catch
		{
			return F_UNDEFINED_VALUE;
		}
	}

	public float GetStorageDeviceVolumeAvailableCapacity(int _index, int _volume_index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(149557716u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetStorageDeviceVolumeAvailableCapacity)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetStorageDeviceVolumeAvailableCapacity)))(objptr, _index, _volume_index);
			}
			return F_UNDEFINED_VALUE;
		}
		catch
		{
			return F_UNDEFINED_VALUE;
		}
	}

	public bool GetStorageDeviceSmartAttribute(int _hdd_index, int _attrib_index, ref int _id, ref int _flags, ref int _value, ref int _worst, byte[] _data)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(851994000u);
			if (intPtr != IntPtr.Zero)
			{
				if (((CPUIDSDK_fp_GetStorageDeviceSmartAttribute)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetStorageDeviceSmartAttribute)))(objptr, _hdd_index, _attrib_index, ref _id, ref _flags, ref _value, ref _worst, _data) == 1)
				{
					return true;
				}
				return false;
			}
			return false;
		}
		catch
		{
			return false;
		}
	}

	public int GetStorageDevicePowerOnHours(int _index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(2674212554u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetStorageDevicePowerOnHours)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetStorageDevicePowerOnHours)))(objptr, _index);
			}
			return I_UNDEFINED_VALUE;
		}
		catch
		{
			return I_UNDEFINED_VALUE;
		}
	}

	public int GetStorageDevicePowerCycleCount(int _index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(1408149469u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetStorageDevicePowerCycleCount)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetStorageDevicePowerCycleCount)))(objptr, _index);
			}
			return I_UNDEFINED_VALUE;
		}
		catch
		{
			return I_UNDEFINED_VALUE;
		}
	}

	public float GetStorageDeviceTotalCapacity(int _index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(2070214345u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetStorageDeviceTotalCapacity)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetStorageDeviceTotalCapacity)))(objptr, _index);
			}
			return F_UNDEFINED_VALUE;
		}
		catch
		{
			return F_UNDEFINED_VALUE;
		}
	}

	public int GetNumberOfDevices()
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(339486840u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetNumberOfDevices)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetNumberOfDevices)))(objptr);
			}
			return I_UNDEFINED_VALUE;
		}
		catch
		{
			return I_UNDEFINED_VALUE;
		}
	}

	public int GetDeviceClass(int _device_index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(948597012u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetDeviceClass)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetDeviceClass)))(objptr, _device_index);
			}
			return I_UNDEFINED_VALUE;
		}
		catch
		{
			return I_UNDEFINED_VALUE;
		}
	}

	public string GetDeviceName(int _device_index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(1611579421u);
			if (intPtr != IntPtr.Zero)
			{
				IntPtr ptr = ((CPUIDSDK_fp_GetDeviceName)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetDeviceName)))(objptr, _device_index);
				string result = Marshal.PtrToStringAnsi(ptr);
				Marshal.FreeBSTR(ptr);
				return result;
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	public string GetDeviceSerialNumber(int _device_index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(873490464u);
			if (intPtr != IntPtr.Zero)
			{
				IntPtr ptr = ((CPUIDSDK_fp_GetDeviceSerialNumber)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetDeviceSerialNumber)))(objptr, _device_index);
				string result = Marshal.PtrToStringAnsi(ptr);
				Marshal.FreeBSTR(ptr);
				return result;
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	public int GetNumberOfSensors(int _device_index, int _sensor_class)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(3426719871u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetNumberOfSensors)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetNumberOfSensors)))(objptr, _device_index, _sensor_class);
			}
			return I_UNDEFINED_VALUE;
		}
		catch
		{
			return I_UNDEFINED_VALUE;
		}
	}

	public bool GetSensorInfos(int _device_index, int _sensor_index, int _sensor_class, ref int _sensor_id, ref string _szName, ref int _raw_value, ref float _value, ref float _min_value, ref float _max_value)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(3633033495u);
			if (intPtr != IntPtr.Zero)
			{
				IntPtr _szNamePtr = IntPtr.Zero;
				if (((CPUIDSDK_fp_GetSensorInfos)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetSensorInfos)))(objptr, _device_index, _sensor_index, _sensor_class, ref _sensor_id, ref _szNamePtr, ref _raw_value, ref _value, ref _min_value, ref _max_value) == 1)
				{
					_szName = Marshal.PtrToStringAnsi(_szNamePtr);
					Marshal.FreeBSTR(_szNamePtr);
					return true;
				}
				return false;
			}
			return false;
		}
		catch
		{
			return false;
		}
	}

	public void SensorClearMinMax(int _device_index, int _sensor_index, int _sensor_class, ref int _sensor_id, ref string _szName, ref int _raw_value, ref float _value, ref float _min_value, ref float _max_value)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(1694812681u);
			if (intPtr != IntPtr.Zero)
			{
				_ = IntPtr.Zero;
				((CPUIDSDK_fp_SensorClearMinMax)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_SensorClearMinMax)))(objptr, _device_index, _sensor_index, _sensor_class);
			}
		}
		catch
		{
		}
	}

	public float GetSensorTypeValue(int _sensor_type, ref int _device_index, ref int _sensor_index)
	{
		try
		{
			IntPtr intPtr = CPUIDSDK_fp_QueryInterface(3135206846u);
			if (intPtr != IntPtr.Zero)
			{
				return ((CPUIDSDK_fp_GetSensorTypeValue)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CPUIDSDK_fp_GetSensorTypeValue)))(objptr, _sensor_type, ref _device_index, ref _sensor_index);
			}
			return F_UNDEFINED_VALUE;
		}
		catch
		{
			return F_UNDEFINED_VALUE;
		}
	}
}
