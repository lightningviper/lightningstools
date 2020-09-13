EESchema Schematic File Version 4
EELAYER 29 0
EELAYER END
$Descr A4 11693 8268
encoding utf-8
Sheet 1 1
Title ""
Date ""
Rev ""
Comp ""
Comment1 ""
Comment2 ""
Comment3 ""
Comment4 ""
$EndDescr
$Comp
L teensy:Teensy3.6 U1
U 1 1 5F5D71AC
P 3000 2200
F 0 "U1" V 3053 -78 60  0000 R CNN
F 1 "Teensy3.6" V 2947 -78 60  0000 R CNN
F 2 "teensy:Teensy35_36" V 2894 -78 60  0001 R CNN
F 3 "" H 3000 2250 60  0000 C CNN
	1    3000 2200
	0    -1   -1   0   
$EndComp
Text GLabel 1050 3600 3    50   Output ~ 0
C1
Text GLabel 1150 3600 3    50   Output ~ 0
C2
Text GLabel 1250 3600 3    50   Output ~ 0
C3
Text GLabel 1350 3600 3    50   Output ~ 0
C4
Text GLabel 1450 3600 3    50   Output ~ 0
C5
Wire Wire Line
	1050 3600 1050 3350
Wire Wire Line
	1150 3600 1150 3350
Wire Wire Line
	1250 3600 1250 3350
Wire Wire Line
	1350 3600 1350 3350
Wire Wire Line
	1450 3600 1450 3350
Text GLabel 1750 3600 3    50   Output ~ 0
VB
Text GLabel 1950 3600 3    50   Output ~ 0
CLK
Text GLabel 2150 3600 3    50   Output ~ 0
DATA
Wire Wire Line
	1750 3600 1750 3350
Wire Wire Line
	1950 3600 1950 3350
Wire Wire Line
	2150 3600 2150 3350
NoConn ~ 950  3350
NoConn ~ 1550 3350
NoConn ~ 1650 3350
NoConn ~ 1850 3350
NoConn ~ 2050 3350
NoConn ~ 2250 3350
NoConn ~ 2350 3350
NoConn ~ 2450 3350
NoConn ~ 2550 3350
NoConn ~ 2650 3350
NoConn ~ 2750 3350
NoConn ~ 2850 3350
NoConn ~ 2950 3350
NoConn ~ 3050 3350
NoConn ~ 3150 3350
NoConn ~ 3250 3350
NoConn ~ 3350 3350
NoConn ~ 3450 3350
NoConn ~ 3550 3350
NoConn ~ 3650 3350
NoConn ~ 3750 3350
NoConn ~ 3850 3350
NoConn ~ 3950 3350
NoConn ~ 4050 3350
NoConn ~ 4150 3350
NoConn ~ 4250 3350
NoConn ~ 4350 3350
NoConn ~ 4450 3350
NoConn ~ 4550 3350
NoConn ~ 4650 3350
NoConn ~ 4750 3350
NoConn ~ 4950 3350
NoConn ~ 5050 3350
NoConn ~ 5150 3350
NoConn ~ 5150 1050
NoConn ~ 5050 1050
NoConn ~ 4950 1050
NoConn ~ 4850 1050
NoConn ~ 4750 1050
NoConn ~ 4650 1050
NoConn ~ 4250 1050
NoConn ~ 4150 1050
NoConn ~ 4050 1050
NoConn ~ 3950 1050
NoConn ~ 3650 1050
NoConn ~ 3550 1050
NoConn ~ 3450 1050
NoConn ~ 3350 1050
NoConn ~ 3250 1050
NoConn ~ 3150 1050
NoConn ~ 3050 1050
NoConn ~ 2950 1050
NoConn ~ 2850 1050
NoConn ~ 2750 1050
NoConn ~ 2650 1050
NoConn ~ 2550 1050
NoConn ~ 2450 1050
NoConn ~ 2350 1050
NoConn ~ 2250 1050
NoConn ~ 2150 1050
NoConn ~ 2050 1050
NoConn ~ 1950 1050
NoConn ~ 1850 1050
NoConn ~ 1750 1050
NoConn ~ 1650 1050
NoConn ~ 1550 1050
NoConn ~ 1450 1050
NoConn ~ 1350 1050
NoConn ~ 1250 1050
NoConn ~ 1150 1050
NoConn ~ 1050 1050
NoConn ~ 950  1050
NoConn ~ 850  1050
NoConn ~ 650  1800
Text GLabel 9600 1250 0    50   Input ~ 0
C1
$Comp
L Device:R R1
U 1 1 5F60D603
P 9900 1250
F 0 "R1" V 9693 1250 50  0000 C CNN
F 1 "120Ω" V 9784 1250 50  0000 C CNN
F 2 "" V 9830 1250 50  0001 C CNN
F 3 "~" H 9900 1250 50  0001 C CNN
	1    9900 1250
	0    1    1    0   
$EndComp
$Comp
L Device:Q_PNP_Darlington_BCE Q1
U 1 1 5F60E145
P 10350 1250
F 0 "Q1" H 10541 1204 50  0000 L CNN
F 1 "TIP127" H 10541 1295 50  0000 L CNN
F 2 "Package_TO_SOT_THT:TO-220-3_Vertical" H 10550 1175 50  0001 L CIN
F 3 "http://www.fairchildsemi.com/ds/TI/TIP125.pdf" H 10350 1250 50  0001 L CNN
	1    10350 1250
	1    0    0    1   
$EndComp
Wire Wire Line
	9600 1250 9750 1250
Wire Wire Line
	10050 1250 10100 1250
Connection ~ 10100 1250
Wire Wire Line
	10100 1250 10150 1250
Wire Wire Line
	10450 600  10450 700 
Connection ~ 10450 700 
Wire Wire Line
	10450 700  10450 1050
$Comp
L hcms-2313:HCMS-2313 Dis1
U 1 1 5F667DCB
P 1050 4500
F 0 "Dis1" H 850 4850 50  0000 L CNN
F 1 "HCMS-2313" H 1650 4850 50  0000 L CNN
F 2 "" H 1050 4500 50  0001 C CNN
F 3 "" H 1050 4500 50  0001 C CNN
	1    1050 4500
	1    0    0    -1  
$EndComp
Text GLabel 900  5300 3    50   Input ~ 0
COL1
Wire Wire Line
	900  5300 900  5150
Text GLabel 1000 5300 3    50   Input ~ 0
COL2
Text GLabel 1100 5300 3    50   Input ~ 0
COL3
Text GLabel 1200 5300 3    50   Input ~ 0
COL4
Text GLabel 1300 5300 3    50   Input ~ 0
COL5
Text GLabel 1600 5300 3    50   Input ~ 0
VB
Text GLabel 1700 5300 3    50   Input ~ 0
+5V
Text GLabel 1800 5300 3    50   Input ~ 0
CLK
Text GLabel 1900 5300 3    50   Output ~ 0
GND
Wire Wire Line
	1000 5300 1000 5150
Wire Wire Line
	1100 5300 1100 5150
Wire Wire Line
	1200 5300 1200 5150
Wire Wire Line
	1300 5300 1300 5150
Wire Wire Line
	1600 5300 1600 5150
Wire Wire Line
	1700 5300 1700 5150
Wire Wire Line
	1800 5300 1800 5150
Wire Wire Line
	1900 5300 1900 5150
Text GLabel 2000 5300 3    50   Input ~ 0
DATA
Wire Wire Line
	2000 5300 2000 5150
$Comp
L hcms-2313:HCMS-2313 Dis2
U 1 1 5F6A7B44
P 2400 4500
F 0 "Dis2" H 2200 4850 50  0000 L CNN
F 1 "HCMS-2313" H 3000 4850 50  0000 L CNN
F 2 "" H 2400 4500 50  0001 C CNN
F 3 "" H 2400 4500 50  0001 C CNN
	1    2400 4500
	1    0    0    -1  
$EndComp
Text GLabel 2250 5300 3    50   Input ~ 0
COL1
Wire Wire Line
	2250 5300 2250 5150
Text GLabel 2350 5300 3    50   Input ~ 0
COL2
Text GLabel 2450 5300 3    50   Input ~ 0
COL3
Text GLabel 2550 5300 3    50   Input ~ 0
COL4
Text GLabel 2650 5300 3    50   Input ~ 0
COL5
Text GLabel 2950 5300 3    50   Input ~ 0
VB
Text GLabel 3050 5300 3    50   Input ~ 0
+5V
Text GLabel 3150 5300 3    50   Input ~ 0
CLK
Text GLabel 3250 5300 3    50   Output ~ 0
GND
Wire Wire Line
	2350 5300 2350 5150
Wire Wire Line
	2450 5300 2450 5150
Wire Wire Line
	2550 5300 2550 5150
Wire Wire Line
	2650 5300 2650 5150
Wire Wire Line
	2950 5300 2950 5150
Wire Wire Line
	3050 5300 3050 5150
Wire Wire Line
	3150 5300 3150 5150
Wire Wire Line
	3250 5300 3250 5150
$Comp
L hcms-2313:HCMS-2313 Dis3
U 1 1 5F6AC71F
P 3750 4500
F 0 "Dis3" H 3550 4850 50  0000 L CNN
F 1 "HCMS-2313" H 4350 4850 50  0000 L CNN
F 2 "" H 3750 4500 50  0001 C CNN
F 3 "" H 3750 4500 50  0001 C CNN
	1    3750 4500
	1    0    0    -1  
$EndComp
Text GLabel 3600 5300 3    50   Input ~ 0
COL1
Wire Wire Line
	3600 5300 3600 5150
Text GLabel 3700 5300 3    50   Input ~ 0
COL2
Text GLabel 3800 5300 3    50   Input ~ 0
COL3
Text GLabel 3900 5300 3    50   Input ~ 0
COL4
Text GLabel 4000 5300 3    50   Input ~ 0
COL5
Text GLabel 4300 5300 3    50   Input ~ 0
VB
Text GLabel 4400 5300 3    50   Input ~ 0
+5V
Text GLabel 4500 5300 3    50   Input ~ 0
CLK
Text GLabel 4600 5300 3    50   Output ~ 0
GND
Wire Wire Line
	3700 5300 3700 5150
Wire Wire Line
	3800 5300 3800 5150
Wire Wire Line
	3900 5300 3900 5150
Wire Wire Line
	4000 5300 4000 5150
Wire Wire Line
	4300 5300 4300 5150
Wire Wire Line
	4400 5300 4400 5150
Wire Wire Line
	4500 5300 4500 5150
Wire Wire Line
	4600 5300 4600 5150
$Comp
L hcms-2313:HCMS-2313 Dis4
U 1 1 5F6AC73D
P 5100 4500
F 0 "Dis4" H 4900 4850 50  0000 L CNN
F 1 "HCMS-2313" H 5700 4850 50  0000 L CNN
F 2 "" H 5100 4500 50  0001 C CNN
F 3 "" H 5100 4500 50  0001 C CNN
	1    5100 4500
	1    0    0    -1  
$EndComp
Text GLabel 4950 5300 3    50   Input ~ 0
COL1
Wire Wire Line
	4950 5300 4950 5150
Text GLabel 5050 5300 3    50   Input ~ 0
COL2
Text GLabel 5150 5300 3    50   Input ~ 0
COL3
Text GLabel 5250 5300 3    50   Input ~ 0
COL4
Text GLabel 5350 5300 3    50   Input ~ 0
COL5
Text GLabel 5650 5300 3    50   Input ~ 0
VB
Text GLabel 5750 5300 3    50   Input ~ 0
+5V
Text GLabel 5850 5300 3    50   Input ~ 0
CLK
Text GLabel 5950 5300 3    50   Output ~ 0
GND
Wire Wire Line
	5050 5300 5050 5150
Wire Wire Line
	5150 5300 5150 5150
Wire Wire Line
	5250 5300 5250 5150
Wire Wire Line
	5350 5300 5350 5150
Wire Wire Line
	5650 5300 5650 5150
Wire Wire Line
	5750 5300 5750 5150
Wire Wire Line
	5850 5300 5850 5150
Wire Wire Line
	5950 5300 5950 5150
$Comp
L hcms-2313:HCMS-2313 Dis5
U 1 1 5F6B58F9
P 1100 6200
F 0 "Dis5" H 900 6550 50  0000 L CNN
F 1 "HCMS-2013" H 1700 6550 50  0000 L CNN
F 2 "" H 1100 6200 50  0001 C CNN
F 3 "" H 1100 6200 50  0001 C CNN
	1    1100 6200
	1    0    0    -1  
$EndComp
Text GLabel 950  7000 3    50   Input ~ 0
COL1
Wire Wire Line
	950  7000 950  6850
Text GLabel 1050 7000 3    50   Input ~ 0
COL2
Text GLabel 1150 7000 3    50   Input ~ 0
COL3
Text GLabel 1250 7000 3    50   Input ~ 0
COL4
Text GLabel 1350 7000 3    50   Input ~ 0
COL5
Text GLabel 1650 7000 3    50   Input ~ 0
VB
Text GLabel 1750 7000 3    50   Input ~ 0
+5V
Text GLabel 1850 7000 3    50   Input ~ 0
CLK
Text GLabel 1950 7000 3    50   Output ~ 0
GND
Wire Wire Line
	1050 7000 1050 6850
Wire Wire Line
	1150 7000 1150 6850
Wire Wire Line
	1250 7000 1250 6850
Wire Wire Line
	1350 7000 1350 6850
Wire Wire Line
	1650 7000 1650 6850
Wire Wire Line
	1750 7000 1750 6850
Wire Wire Line
	1850 7000 1850 6850
Wire Wire Line
	1950 7000 1950 6850
$Comp
L hcms-2313:HCMS-2313 Dis6
U 1 1 5F6B5917
P 2450 6200
F 0 "Dis6" H 2250 6550 50  0000 L CNN
F 1 "HCMS-2013" H 3050 6550 50  0000 L CNN
F 2 "" H 2450 6200 50  0001 C CNN
F 3 "" H 2450 6200 50  0001 C CNN
	1    2450 6200
	1    0    0    -1  
$EndComp
Text GLabel 2300 7000 3    50   Input ~ 0
COL1
Wire Wire Line
	2300 7000 2300 6850
Text GLabel 2400 7000 3    50   Input ~ 0
COL2
Text GLabel 2500 7000 3    50   Input ~ 0
COL3
Text GLabel 2600 7000 3    50   Input ~ 0
COL4
Text GLabel 2700 7000 3    50   Input ~ 0
COL5
Text GLabel 3000 7000 3    50   Input ~ 0
VB
Text GLabel 3100 7000 3    50   Input ~ 0
+5V
Text GLabel 3200 7000 3    50   Input ~ 0
CLK
Text GLabel 3300 7000 3    50   Output ~ 0
GND
Wire Wire Line
	2400 7000 2400 6850
Wire Wire Line
	2500 7000 2500 6850
Wire Wire Line
	2600 7000 2600 6850
Wire Wire Line
	2700 7000 2700 6850
Wire Wire Line
	3000 7000 3000 6850
Wire Wire Line
	3100 7000 3100 6850
Wire Wire Line
	3200 7000 3200 6850
Wire Wire Line
	3300 7000 3300 6850
$Comp
L hcms-2313:HCMS-2313 Dis7
U 1 1 5F6B5935
P 3800 6200
F 0 "Dis7" H 3600 6550 50  0000 L CNN
F 1 "HCMS-2013" H 4400 6550 50  0000 L CNN
F 2 "" H 3800 6200 50  0001 C CNN
F 3 "" H 3800 6200 50  0001 C CNN
	1    3800 6200
	1    0    0    -1  
$EndComp
Text GLabel 3650 7000 3    50   Input ~ 0
COL1
Wire Wire Line
	3650 7000 3650 6850
Text GLabel 3750 7000 3    50   Input ~ 0
COL2
Text GLabel 3850 7000 3    50   Input ~ 0
COL3
Text GLabel 3950 7000 3    50   Input ~ 0
COL4
Text GLabel 4050 7000 3    50   Input ~ 0
COL5
Text GLabel 4350 7000 3    50   Input ~ 0
VB
Text GLabel 4450 7000 3    50   Input ~ 0
+5V
Text GLabel 4550 7000 3    50   Input ~ 0
CLK
Text GLabel 4650 7000 3    50   Output ~ 0
GND
Wire Wire Line
	3750 7000 3750 6850
Wire Wire Line
	3850 7000 3850 6850
Wire Wire Line
	3950 7000 3950 6850
Wire Wire Line
	4050 7000 4050 6850
Wire Wire Line
	4350 7000 4350 6850
Wire Wire Line
	4450 7000 4450 6850
Wire Wire Line
	4550 7000 4550 6850
Wire Wire Line
	4650 7000 4650 6850
$Comp
L hcms-2313:HCMS-2313 Dis8
U 1 1 5F6B5953
P 5150 6200
F 0 "Dis8" H 4950 6550 50  0000 L CNN
F 1 "HCMS-2013" H 5750 6550 50  0000 L CNN
F 2 "" H 5150 6200 50  0001 C CNN
F 3 "" H 5150 6200 50  0001 C CNN
	1    5150 6200
	1    0    0    -1  
$EndComp
Text GLabel 5000 7000 3    50   Input ~ 0
COL1
Wire Wire Line
	5000 7000 5000 6850
Text GLabel 5100 7000 3    50   Input ~ 0
COL2
Text GLabel 5200 7000 3    50   Input ~ 0
COL3
Text GLabel 5300 7000 3    50   Input ~ 0
COL4
Text GLabel 5400 7000 3    50   Input ~ 0
COL5
Text GLabel 5700 7000 3    50   Input ~ 0
VB
Text GLabel 5800 7000 3    50   Input ~ 0
+5V
Text GLabel 5900 7000 3    50   Input ~ 0
CLK
Text GLabel 6000 7000 3    50   Output ~ 0
GND
Wire Wire Line
	5100 7000 5100 6850
Wire Wire Line
	5200 7000 5200 6850
Wire Wire Line
	5300 7000 5300 6850
Wire Wire Line
	5400 7000 5400 6850
Wire Wire Line
	5700 7000 5700 6850
Wire Wire Line
	5800 7000 5800 6850
Wire Wire Line
	5900 7000 5900 6850
Wire Wire Line
	6000 7000 6000 6850
Wire Wire Line
	1500 5150 1500 5650
Wire Wire Line
	1500 5650 3350 5650
Wire Wire Line
	3350 5650 3350 5150
Wire Wire Line
	2850 5150 2850 5600
Wire Wire Line
	2850 5600 4700 5600
Wire Wire Line
	4700 5600 4700 5150
Wire Wire Line
	4200 5150 4200 5650
Wire Wire Line
	4200 5650 6050 5650
Wire Wire Line
	6050 5650 6050 5150
Wire Wire Line
	5550 5150 5550 5750
Wire Wire Line
	5550 5750 750  5750
Wire Wire Line
	750  5750 750  7300
Wire Wire Line
	750  7300 2050 7300
Wire Wire Line
	2050 7300 2050 6850
Text GLabel 850  3600 3    50   Output ~ 0
GND
Wire Wire Line
	850  3600 850  3350
Text GLabel 4350 900  1    50   Input ~ 0
+5V
Wire Wire Line
	4350 1050 4350 900 
Text Notes 7500 7500 0    50   ~ 0
EWMU Character Display Driver
Text Notes 8200 7650 0    50   ~ 0
9/13/2020
Text Notes 10650 7650 0    50   ~ 0
0.1
Text Notes 10445 1075 3    50   ~ 0
C
Text Notes 10440 1380 3    50   ~ 0
E
Text Notes 10230 1250 0    50   ~ 0
B
Text GLabel 10600 600  2    50   Input ~ 0
+5V
Wire Wire Line
	10600 600  10450 600 
Text GLabel 10650 1550 2    50   Output ~ 0
COL1
Wire Wire Line
	10450 1450 10450 1550
Wire Wire Line
	10450 1550 10650 1550
Wire Wire Line
	10100 700  10450 700 
Wire Wire Line
	10100 1000 10100 1250
$Comp
L Device:R R6
U 1 1 5F61BE65
P 10100 850
F 0 "R6" H 10030 804 50  0000 R CNN
F 1 "2.7KΩ" H 10030 895 50  0000 R CNN
F 2 "" V 10030 850 50  0001 C CNN
F 3 "~" H 10100 850 50  0001 C CNN
	1    10100 850 
	-1   0    0    1   
$EndComp
Text GLabel 9600 2400 0    50   Input ~ 0
C2
$Comp
L Device:R R2
U 1 1 5F833B19
P 9900 2400
F 0 "R2" V 9693 2400 50  0000 C CNN
F 1 "120Ω" V 9784 2400 50  0000 C CNN
F 2 "" V 9830 2400 50  0001 C CNN
F 3 "~" H 9900 2400 50  0001 C CNN
	1    9900 2400
	0    1    1    0   
$EndComp
$Comp
L Device:Q_PNP_Darlington_BCE Q2
U 1 1 5F833B23
P 10350 2400
F 0 "Q2" H 10541 2354 50  0000 L CNN
F 1 "TIP127" H 10541 2445 50  0000 L CNN
F 2 "Package_TO_SOT_THT:TO-220-3_Vertical" H 10550 2325 50  0001 L CIN
F 3 "http://www.fairchildsemi.com/ds/TI/TIP125.pdf" H 10350 2400 50  0001 L CNN
	1    10350 2400
	1    0    0    1   
$EndComp
Wire Wire Line
	9600 2400 9750 2400
Wire Wire Line
	10050 2400 10100 2400
Connection ~ 10100 2400
Wire Wire Line
	10100 2400 10150 2400
Wire Wire Line
	10450 1750 10450 1850
Connection ~ 10450 1850
Wire Wire Line
	10450 1850 10450 2200
Text Notes 10445 2225 3    50   ~ 0
C
Text Notes 10440 2530 3    50   ~ 0
E
Text Notes 10230 2400 0    50   ~ 0
B
Text GLabel 10600 1750 2    50   Input ~ 0
+5V
Wire Wire Line
	10600 1750 10450 1750
Text GLabel 10650 2700 2    50   Output ~ 0
COL2
Wire Wire Line
	10450 2600 10450 2700
Wire Wire Line
	10450 2700 10650 2700
Wire Wire Line
	10100 1850 10450 1850
Wire Wire Line
	10100 2150 10100 2400
$Comp
L Device:R R7
U 1 1 5F833B3E
P 10100 2000
F 0 "R7" H 10030 1954 50  0000 R CNN
F 1 "2.7KΩ" H 10030 2045 50  0000 R CNN
F 2 "" V 10030 2000 50  0001 C CNN
F 3 "~" H 10100 2000 50  0001 C CNN
	1    10100 2000
	-1   0    0    1   
$EndComp
Text GLabel 9600 3550 0    50   Input ~ 0
C3
$Comp
L Device:R R3
U 1 1 5F83A769
P 9900 3550
F 0 "R3" V 9693 3550 50  0000 C CNN
F 1 "120Ω" V 9784 3550 50  0000 C CNN
F 2 "" V 9830 3550 50  0001 C CNN
F 3 "~" H 9900 3550 50  0001 C CNN
	1    9900 3550
	0    1    1    0   
$EndComp
$Comp
L Device:Q_PNP_Darlington_BCE Q3
U 1 1 5F83A773
P 10350 3550
F 0 "Q3" H 10541 3504 50  0000 L CNN
F 1 "TIP127" H 10541 3595 50  0000 L CNN
F 2 "Package_TO_SOT_THT:TO-220-3_Vertical" H 10550 3475 50  0001 L CIN
F 3 "http://www.fairchildsemi.com/ds/TI/TIP125.pdf" H 10350 3550 50  0001 L CNN
	1    10350 3550
	1    0    0    1   
$EndComp
Wire Wire Line
	9600 3550 9750 3550
Wire Wire Line
	10050 3550 10100 3550
Connection ~ 10100 3550
Wire Wire Line
	10100 3550 10150 3550
Wire Wire Line
	10450 2900 10450 3000
Connection ~ 10450 3000
Wire Wire Line
	10450 3000 10450 3350
Text Notes 10445 3375 3    50   ~ 0
C
Text Notes 10440 3680 3    50   ~ 0
E
Text Notes 10230 3550 0    50   ~ 0
B
Text GLabel 10600 2900 2    50   Input ~ 0
+5V
Wire Wire Line
	10600 2900 10450 2900
Text GLabel 10650 3850 2    50   Output ~ 0
COL3
Wire Wire Line
	10450 3750 10450 3850
Wire Wire Line
	10450 3850 10650 3850
Wire Wire Line
	10100 3000 10450 3000
Wire Wire Line
	10100 3300 10100 3550
$Comp
L Device:R R8
U 1 1 5F83A78E
P 10100 3150
F 0 "R8" H 10030 3104 50  0000 R CNN
F 1 "2.7KΩ" H 10030 3195 50  0000 R CNN
F 2 "" V 10030 3150 50  0001 C CNN
F 3 "~" H 10100 3150 50  0001 C CNN
	1    10100 3150
	-1   0    0    1   
$EndComp
Text GLabel 9600 4700 0    50   Input ~ 0
C4
$Comp
L Device:R R4
U 1 1 5F8416DF
P 9900 4700
F 0 "R4" V 9693 4700 50  0000 C CNN
F 1 "120Ω" V 9784 4700 50  0000 C CNN
F 2 "" V 9830 4700 50  0001 C CNN
F 3 "~" H 9900 4700 50  0001 C CNN
	1    9900 4700
	0    1    1    0   
$EndComp
$Comp
L Device:Q_PNP_Darlington_BCE Q4
U 1 1 5F8416E9
P 10350 4700
F 0 "Q4" H 10541 4654 50  0000 L CNN
F 1 "TIP127" H 10541 4745 50  0000 L CNN
F 2 "Package_TO_SOT_THT:TO-220-3_Vertical" H 10550 4625 50  0001 L CIN
F 3 "http://www.fairchildsemi.com/ds/TI/TIP125.pdf" H 10350 4700 50  0001 L CNN
	1    10350 4700
	1    0    0    1   
$EndComp
Wire Wire Line
	9600 4700 9750 4700
Wire Wire Line
	10050 4700 10100 4700
Connection ~ 10100 4700
Wire Wire Line
	10100 4700 10150 4700
Wire Wire Line
	10450 4050 10450 4150
Connection ~ 10450 4150
Wire Wire Line
	10450 4150 10450 4500
Text Notes 10445 4525 3    50   ~ 0
C
Text Notes 10440 4830 3    50   ~ 0
E
Text Notes 10230 4700 0    50   ~ 0
B
Text GLabel 10600 4050 2    50   Input ~ 0
+5V
Wire Wire Line
	10600 4050 10450 4050
Text GLabel 10650 5000 2    50   Output ~ 0
COL4
Wire Wire Line
	10450 4900 10450 5000
Wire Wire Line
	10450 5000 10650 5000
Wire Wire Line
	10100 4150 10450 4150
Wire Wire Line
	10100 4450 10100 4700
$Comp
L Device:R R9
U 1 1 5F841704
P 10100 4300
F 0 "R9" H 10030 4254 50  0000 R CNN
F 1 "2.7KΩ" H 10030 4345 50  0000 R CNN
F 2 "" V 10030 4300 50  0001 C CNN
F 3 "~" H 10100 4300 50  0001 C CNN
	1    10100 4300
	-1   0    0    1   
$EndComp
Text GLabel 9600 5850 0    50   Input ~ 0
C5
$Comp
L Device:R R5
U 1 1 5F848CD7
P 9900 5850
F 0 "R5" V 9693 5850 50  0000 C CNN
F 1 "120Ω" V 9784 5850 50  0000 C CNN
F 2 "" V 9830 5850 50  0001 C CNN
F 3 "~" H 9900 5850 50  0001 C CNN
	1    9900 5850
	0    1    1    0   
$EndComp
$Comp
L Device:Q_PNP_Darlington_BCE Q5
U 1 1 5F848CE1
P 10350 5850
F 0 "Q5" H 10541 5804 50  0000 L CNN
F 1 "TIP127" H 10541 5895 50  0000 L CNN
F 2 "Package_TO_SOT_THT:TO-220-3_Vertical" H 10550 5775 50  0001 L CIN
F 3 "http://www.fairchildsemi.com/ds/TI/TIP125.pdf" H 10350 5850 50  0001 L CNN
	1    10350 5850
	1    0    0    1   
$EndComp
Wire Wire Line
	9600 5850 9750 5850
Wire Wire Line
	10050 5850 10100 5850
Connection ~ 10100 5850
Wire Wire Line
	10100 5850 10150 5850
Wire Wire Line
	10450 5200 10450 5300
Connection ~ 10450 5300
Wire Wire Line
	10450 5300 10450 5650
Text Notes 10445 5675 3    50   ~ 0
C
Text Notes 10440 5980 3    50   ~ 0
E
Text Notes 10230 5850 0    50   ~ 0
B
Text GLabel 10600 5200 2    50   Input ~ 0
+5V
Wire Wire Line
	10600 5200 10450 5200
Text GLabel 10650 6150 2    50   Output ~ 0
COL5
Wire Wire Line
	10450 6050 10450 6150
Wire Wire Line
	10450 6150 10650 6150
Wire Wire Line
	10100 5300 10450 5300
Wire Wire Line
	10100 5600 10100 5850
$Comp
L Device:R R10
U 1 1 5F848CFC
P 10100 5450
F 0 "R10" H 10030 5404 50  0000 R CNN
F 1 "2.7KΩ" H 10030 5495 50  0000 R CNN
F 2 "" V 10030 5450 50  0001 C CNN
F 3 "~" H 10100 5450 50  0001 C CNN
	1    10100 5450
	-1   0    0    1   
$EndComp
Text Notes 9300 1100 2    50   ~ 0
EXTERNAL CONNECTOR
Wire Notes Line
	6350 500  6350 3850
Wire Notes Line
	600  3850 600  500 
Text Notes 6300 3800 2    50   ~ 0
MICROCONTROLLER
Wire Notes Line
	6350 7500 600  7500
Wire Notes Line
	600  3950 6350 3950
Text Notes 6300 7450 2    50   ~ 0
EWMU DISPLAYS
Wire Notes Line
	11100 6450 11100 500 
Text Notes 11000 6400 2    50   ~ 0
DISPLAY COLUMN DRIVERS
$Comp
L Device:R_POT RV1
U 1 1 5F8F865F
P 6970 1815
F 0 "RV1" H 6900 1861 50  0000 R CNN
F 1 "90KΩ" H 6900 1770 50  0000 R CNN
F 2 "" H 6970 1815 50  0001 C CNN
F 3 "~" H 6970 1815 50  0001 C CNN
	1    6970 1815
	1    0    0    -1  
$EndComp
Text GLabel 4850 3600 3    50   Input ~ 0
BRT
Wire Wire Line
	4850 3600 4850 3350
Text GLabel 7270 1815 2    50   Output ~ 0
BRT
Wire Wire Line
	7120 1815 7270 1815
Text GLabel 4450 900  1    50   Output ~ 0
AGND
Text GLabel 4550 900  1    50   Output ~ 0
+3.3V
Wire Wire Line
	4450 1050 4450 900 
Wire Wire Line
	4550 1050 4550 900 
Text GLabel 6970 1565 1    50   Input ~ 0
+3.3V
Text GLabel 6970 2165 3    50   Input ~ 0
AGND
Wire Wire Line
	6970 1565 6970 1665
Wire Wire Line
	6970 2165 6970 1965
Wire Notes Line
	6450 1200 9350 1200
Wire Notes Line
	9350 1200 9350 2500
Wire Notes Line
	9350 2500 6450 2500
Wire Notes Line
	6450 2500 6450 1200
Text Notes 8500 2450 0    50   ~ 0
BRIGHTNESS CONTROL
Wire Notes Line
	600  500  6350 500 
Wire Notes Line
	600  3850 6350 3850
Wire Notes Line
	6350 3950 6350 7500
Wire Notes Line
	600  3950 600  7500
Wire Notes Line
	6450 500  6450 1150
Wire Notes Line
	6450 1150 9350 1150
Wire Notes Line
	9350 1150 9350 500 
Wire Notes Line
	11100 500  9400 500 
Wire Notes Line
	9400 6450 11100 6450
Wire Notes Line
	9400 500  9400 6450
Text Notes 1200 900  0    79   ~ 0
NOTE: CUT TRACE ON BACK OF TEENSY \nTO SEPARATE VIN FROM VUSB
Wire Wire Line
	1550 6850 1550 7350
Wire Wire Line
	1550 7350 3400 7350
Wire Wire Line
	3400 7350 3400 6850
Wire Wire Line
	2900 6850 2900 7300
Wire Wire Line
	2900 7300 4750 7300
Wire Wire Line
	4750 7300 4750 6850
Wire Wire Line
	4250 6850 4250 7350
Wire Wire Line
	4250 7350 6100 7350
Wire Wire Line
	6100 7350 6100 6850
NoConn ~ 5600 6850
Text Notes 7650 1100 2    50   ~ 0
POWER SUPPLY: 5VDC, >=1A
$Comp
L Connector_Generic:Conn_01x02 J1
U 1 1 5F78F54A
P 7100 700
F 0 "J1" H 7180 692 50  0000 L CNN
F 1 "Conn_01x02" H 7180 601 50  0000 L CNN
F 2 "" H 7100 700 50  0001 C CNN
F 3 "~" H 7100 700 50  0001 C CNN
	1    7100 700 
	1    0    0    -1  
$EndComp
Text GLabel 6750 700  0    50   Input ~ 0
+5V
Text GLabel 6750 800  0    50   Input ~ 0
GND
Wire Wire Line
	6750 700  6900 700 
Wire Wire Line
	6750 800  6900 800 
Wire Notes Line
	9350 500  6450 500 
$EndSCHEMATC
