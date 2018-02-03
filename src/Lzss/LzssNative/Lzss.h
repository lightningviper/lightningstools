#ifndef LZSS_H
#define LZSS_H

#include "LZSSopt.h"


// This is the maxmimum number (actually, just a very high guess) of additional
// bytes which may be added to an already compressed data stream. 
// I would recomend adding this to your output_string's length before calling either 
// Compress or Expand in order to prevent overwritting the string.
#define MAX_POSSIBLE_OVERWRITE    100

#ifdef C_LINKAGE
#if __cplusplus
extern "C"
{
#endif
#endif

__declspec(dllexport) extern int LZSS_Compress(unsigned char *input_string, unsigned char *output_string, int uncompSize);

__declspec(dllexport) extern int LZSS_Expand(unsigned char *input_string, unsigned char *output_string, int uncompSize);

#ifdef C_LINKAGE
#if __cplusplus
} // extern "C"
#endif
#endif

#endif
