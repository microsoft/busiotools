/* Copyright (c) Microsoft Corporation. All rights reserved.
   Licensed under the MIT License. */

#include "Model_3201.h"

#define EEPROM_SELECT   A0 // PF[0]
#define CDONE           A1 // PF[1]
#define CRESET_B        A2 // PF[2]
#define INIT_DONE       A3 // PF[3]
#define EEPROM_CS       53 // PB[0]

void Model_3201_setup()
{
    digitalWrite( CRESET_B, LOW );
    pinMode( CRESET_B, OUTPUT );
    
	//
	// TODO: Placeholder for FPGA Interface
	//
}

