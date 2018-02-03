
#include <stdafx.h>
#include <stddef.h>
#include <memory.h>
#include "lzss.h"

typedef unsigned char uchar;

#define INDEX_BIT_COUNT      12
#define LENGTH_BIT_COUNT     4
#define WINDOW_SIZE          ( 1 << INDEX_BIT_COUNT )
#define RAW_LOOK_AHEAD_SIZE  ( 1 << LENGTH_BIT_COUNT )
#define BREAK_EVEN           ( ( 1 + INDEX_BIT_COUNT + LENGTH_BIT_COUNT ) / 9 )
#define LOOK_AHEAD_SIZE      ( RAW_LOOK_AHEAD_SIZE + BREAK_EVEN )
#define TREE_ROOT            WINDOW_SIZE
#define END_OF_STREAM        0
#define UNUSED               0
#define MOD_WINDOW( a )      ( ( a ) & ( WINDOW_SIZE - 1 ) )

typedef struct  
	{
 	unsigned char window[ WINDOW_SIZE ];
	struct 
		{
		int parent;
		int smaller_child;
		int larger_child;
		} tree[ WINDOW_SIZE + 1 ];
	unsigned char DataBuffer[ 17 ];
	int FlagBitMask;
	unsigned int BufferOffset;
	unsigned int OldBufferOffset;
	int original_size; 
	int compressed_size;
	int inc_input_string;
	int inc_output_string;
	} LZSS_COMP_CTXT;

void InitTree( int r, LZSS_COMP_CTXT* ctxt );
void ContractNode( int old_node, int new_node, LZSS_COMP_CTXT* ctxt );
void ReplaceNode( int old_node, int new_node, LZSS_COMP_CTXT* ctxt );
int FindNextNode( int node, LZSS_COMP_CTXT* ctxt );
void DeleteString( int p, LZSS_COMP_CTXT* ctxt );
int AddString( int new_node, int *match_position, LZSS_COMP_CTXT* ctxt );
void InitOutputBuffer( LZSS_COMP_CTXT* ctxt );
int FlushOutputBuffer( uchar *output_string, LZSS_COMP_CTXT* ctxt);
int OutputChar( int data, uchar *output_string, LZSS_COMP_CTXT* ctxt );
int OutputPair( int position, int length, uchar *output_string, LZSS_COMP_CTXT* ctxt );
void InitInputBuffer( uchar *input_string, LZSS_COMP_CTXT* ctxt);
int InputBit( uchar *input_string, LZSS_COMP_CTXT* ctxt);

void InitTree (int r, LZSS_COMP_CTXT* ctxt)
{
    int i;

    for ( i = 0 ; i < ( WINDOW_SIZE + 1 ) ; i++ ) {
        ctxt->tree[ i ].parent = UNUSED;
        ctxt->tree[ i ].larger_child = UNUSED;
        ctxt->tree[ i ].smaller_child = UNUSED;
    }
    ctxt->tree[ TREE_ROOT ].larger_child = r;
    ctxt->tree[ r ].parent = TREE_ROOT;
    ctxt->tree[ r ].larger_child = UNUSED;
    ctxt->tree[ r ].smaller_child = UNUSED;
}

void ContractNode(int old_node, int new_node, LZSS_COMP_CTXT* ctxt )
{
    ctxt->tree[ new_node ].parent = ctxt->tree[ old_node ].parent;
    if ( ctxt->tree[ ctxt->tree[ old_node ].parent ].larger_child == old_node )
        ctxt->tree[ ctxt->tree[ old_node ].parent ].larger_child = new_node;
    else
        ctxt->tree[ ctxt->tree[ old_node ].parent ].smaller_child = new_node;
    ctxt->tree[ old_node ].parent = UNUSED;
}

void ReplaceNode(int old_node, int new_node, LZSS_COMP_CTXT* ctxt )
{
    int parent;

    parent = ctxt->tree[ old_node ].parent;
    if ( ctxt->tree[ parent ].smaller_child == old_node )
        ctxt->tree[ parent ].smaller_child = new_node;
    else
        ctxt->tree[ parent ].larger_child = new_node;
    ctxt->tree[ new_node ] = ctxt->tree[ old_node ];
    ctxt->tree[ ctxt->tree[ new_node ].smaller_child ].parent = new_node;
    ctxt->tree[ ctxt->tree[ new_node ].larger_child ].parent = new_node;
    ctxt->tree[ old_node ].parent = UNUSED;
}

int FindNextNode (int node, LZSS_COMP_CTXT* ctxt)
{
    int next;

    next = ctxt->tree[ node ].smaller_child;
    while ( ctxt->tree[ next ].larger_child != UNUSED )
        next = ctxt->tree[ next ].larger_child;
    return( next );
}

void DeleteString (int p, LZSS_COMP_CTXT* ctxt)
{
    int  replacement;

    if ( ctxt->tree[ p ].parent == UNUSED )
        return;
    if ( ctxt->tree[ p ].larger_child == UNUSED )
        ContractNode( p, ctxt->tree[ p ].smaller_child, ctxt );
    else if ( ctxt->tree[ p ].smaller_child == UNUSED )
        ContractNode( p, ctxt->tree[ p ].larger_child, ctxt );
    else {
        replacement = FindNextNode( p, ctxt );
        DeleteString( replacement, ctxt );
        ReplaceNode( p, replacement, ctxt );
    }
}

int AddString (int new_node, int* match_position, LZSS_COMP_CTXT* ctxt)
{
    int i=0;
    int test_node=0;
    int delta=0;
    int match_length=0;
    int *child=NULL;

    if ( new_node == END_OF_STREAM )
        return( 0 );
    test_node = ctxt->tree[ TREE_ROOT ].larger_child;
    match_length = 0;
    for ( ; ; ) {
        for ( i = 0 ; i < LOOK_AHEAD_SIZE ; i++ ) {
            delta = ctxt->window[ MOD_WINDOW( new_node + i ) ] -
                    ctxt->window[ MOD_WINDOW( test_node + i ) ];
            if ( delta != 0 )
                break;
        }
        if ( i >= match_length ) {
            match_length = i;
            *match_position = test_node;
            if ( match_length >= LOOK_AHEAD_SIZE ) {
                ReplaceNode( test_node, new_node, ctxt );
                return( match_length );
            }
        }
        if ( delta >= 0 )
            child = &ctxt->tree[ test_node ].larger_child;
        else
            child = &ctxt->tree[ test_node ].smaller_child;
        if ( *child == UNUSED ) {
            *child = new_node;
            ctxt->tree[ new_node ].parent = test_node;
            ctxt->tree[ new_node ].larger_child = UNUSED;
            ctxt->tree[ new_node ].smaller_child = UNUSED;
            return( match_length );
        }
        test_node = *child;
    }
}

void InitOutputBuffer(LZSS_COMP_CTXT* ctxt)
{
    ctxt->DataBuffer[ 0 ] = 0;
    ctxt->FlagBitMask = 1;
    ctxt->OldBufferOffset = ctxt->BufferOffset ;
    ctxt->BufferOffset = 1;
}

int FlushOutputBuffer(uchar *output_string, LZSS_COMP_CTXT* ctxt)
{
    if ( ctxt->BufferOffset == 1 )
        return( 1 );
    memcpy( output_string, ctxt->DataBuffer, ctxt->BufferOffset ) ;         /**/
    ctxt->compressed_size += ctxt->BufferOffset;                            /**/
    InitOutputBuffer(ctxt);
    return( 1 );
}

int OutputChar (int data, uchar *output_string, LZSS_COMP_CTXT* ctxt)
{
    ctxt->DataBuffer[ ctxt->BufferOffset++ ] = (uchar) data;
    ctxt->DataBuffer[ 0 ] |= ctxt->FlagBitMask;
    ctxt->FlagBitMask <<= 1;
    ctxt->inc_output_string=0;                                /**/
    if ( ctxt->FlagBitMask == 0x100 ){
        ctxt->inc_output_string=1;                            /**/
        return( FlushOutputBuffer(output_string,ctxt) );
    } else
        return( 1 );
}

int OutputPair( int position, int length, uchar *output_string, LZSS_COMP_CTXT* ctxt)
{
    ctxt->DataBuffer[ ctxt->BufferOffset ] = (uchar) ( length << 4 );
    ctxt->DataBuffer[ ctxt->BufferOffset++ ] |= ( position >> 8 );
    ctxt->DataBuffer[ ctxt->BufferOffset++ ] = (uchar) ( position & 0xff );
    ctxt->FlagBitMask <<= 1;
    ctxt->inc_output_string=0;                                /**/
    if ( ctxt->FlagBitMask == 0x100 ){
        ctxt->inc_output_string=1;                            /**/
        return( FlushOutputBuffer(output_string,ctxt) );
    } else
        return( 1 );
}



void InitInputBuffer (uchar *input_string, LZSS_COMP_CTXT* ctxt)				/**/
{
    ctxt->FlagBitMask = 1;
    ctxt->DataBuffer[ 0 ] = *input_string;										/**/
}

/*
 * When the Expansion program wants a flag bit, it calls this routine.
 * This routine has to keep track of whether or not it has run out of
 * flag bits.  If it has, it has to go back and reinitialize so as to
 * have a fresh set.
 */

int InputBit (uchar *input_string, LZSS_COMP_CTXT* ctxt)						/**/
{
    ctxt->inc_input_string=0;
    if ( ctxt->FlagBitMask == 0x100 ) {
        InitInputBuffer(input_string,ctxt);										/**/
        ctxt->inc_input_string=1;
        }
    ctxt->FlagBitMask <<= 1;
    return( ctxt->DataBuffer[ 0 ] & ( ctxt->FlagBitMask >> 1 ) );
}

/*
 * This is the compression routine.  It has to first load up the look
 * ahead buffer, then go into the main compression loop.  The main loop
 * decides whether to output a single character or an index/length
 * token that defines a phrase.  Once the character or phrase has been
 * sent out, another loop has to run.  The second loop reads in new
 * characters, deletes the strings that are overwritten by the new
 * character, then adds the strings that are created by the new
 * character.  While running it has the additional responsibility of
 * creating the checksum of the input data, and checking for when the
 * output data grows too large.  The program returns a success or failure
 * indicator.  
 *
 */
/*-----------------------------------------------------------------------
        The changes I have made to this function are marked by an empty
   comment.  All of the CRC data stuff has been deleted.
        The output_string now gets sent to all the functions that
   call FlushOuputBuffer, and in FlushOutputBuffer, the buffer is catted
   to the output_string.  Also, where before a call to getc was used to
   fill the window array, I have substituted 
                c= *input;
                input++;
   In addition, the size parameter is decremented everytime the window
   array is filled.  Once  size<=0, the exit condition is initiated
   (look_ahead_bites => 0).
   
   Dave Lewak, 7/25/96
-----------------------------------------------------------------------*/

#ifdef C_LINKAGE
extern "C"
{
#endif

int LZSS_Compress (uchar *input_string, uchar *output_string, int size)
{
    int i;
    uchar c;
    int look_ahead_bytes;
    int current_position;
    int replace_count;
    int match_length;
    int match_position;
	LZSS_COMP_CTXT ctxt;

    ctxt.compressed_size = 0;                                /**/
    ctxt.original_size = size;                               /**/
    InitOutputBuffer(&ctxt);

    current_position = 1;
    for ( i = 0 ; i < LOOK_AHEAD_SIZE ; i++ ) {
        c=*input_string;                                /**/
        input_string++;                                 /**/
        size--;                                         /**/
        if ( size<0)                                    /**/
            break;
        ctxt.window[ current_position + i ] = c;
    }
    look_ahead_bytes = i;
    InitTree( current_position, &ctxt );
    match_length = 0;
    match_position = 0;
    while ( look_ahead_bytes > 0 ) {
        if ( match_length > look_ahead_bytes ){
            match_length = look_ahead_bytes;
		}
        if ( match_length <= BREAK_EVEN ) {
            replace_count = 1;
            if ( !OutputChar( ctxt.window[ current_position ], output_string, &ctxt ) ){
                return( 0 );
			}
            if(ctxt.inc_output_string){
                output_string+=ctxt.OldBufferOffset;
			}
        } else {
            if ( !OutputPair( match_position, 
                    match_length - ( BREAK_EVEN + 1 ), output_string, &ctxt ) ){
                return( 0 );
			}
            if(ctxt.inc_output_string){
                output_string+=ctxt.OldBufferOffset;
			}
            replace_count = match_length;
        }
        for ( i = 0 ; i < replace_count ; i++ ) {
            DeleteString( MOD_WINDOW( current_position + LOOK_AHEAD_SIZE ), &ctxt );
            c=*input_string;  		                    /**/
            size--;                                     /**/
            if ( size<0) {                              /**/
                look_ahead_bytes--;
            } else {
				//Only increment while the end of the input string 
				//hasn't been reached
	            input_string++;                             /**/

                ctxt.window[ MOD_WINDOW( current_position + LOOK_AHEAD_SIZE ) ] = c;
            }
            current_position = MOD_WINDOW( current_position + 1 );
            if ( look_ahead_bytes )
                match_length = AddString( current_position, &match_position, &ctxt );
        }
    }


    /* If the previous OutputChar or OutputPair call
       didn't write to the output, do so now */
    if(!ctxt.inc_output_string)
        FlushOutputBuffer(output_string, &ctxt);

    return( ctxt.compressed_size );
}

/*
 * This is the expansion routine for the LZSS algorithm.  All it has
 * to do is read in flag bits, decide whether to read in a character or
 * a index/length pair, and take the appropriate action.  It is responsible
 * for keeping track of the crc of the output data, and must return it
 * to the calling routine, for verification.
 */

int LZSS_Expand (uchar *input_string, uchar *output_string, int size)
{
    int i;
    int current_position;
    uchar c;
    int match_length;
    int match_position;
    unsigned long input_count;
    uchar *inputHead;
	LZSS_COMP_CTXT ctxt;

    inputHead=input_string;
    InitInputBuffer(input_string, &ctxt);               /**/
    input_string++;                                     /**/
    current_position = 1;

	// While we still have room in the output buffer
    while ( size ) {
        if ( InputBit(input_string, &ctxt) ) {
			// We're going to write a single characters

            /* InputBit if calls InitInputBuffer, 
               then increment input_string */
            if(ctxt.inc_input_string==1)                /**/
                input_string++;                         /**/
            c = *input_string;                          /**/

            /* Exit Condition */
        //    if(c==0)                                  /**/
        //      break;                                  /**/

            input_string++;                             /**/
            *output_string=c;                           /**/
            output_string++;                            /**/
            size--;
            ctxt.window[ current_position ] = c;
            current_position = MOD_WINDOW( current_position + 1 );
        } else {
			// We're going to write a match from the code book

            if(ctxt.inc_input_string==1)                /**/
                input_string++;                         /**/
            match_length = *input_string;               /**/
            input_string++;                             /**/
            match_position = *input_string;             /**/
            input_string++;                             /**/
            match_position |= ( match_length & 0xf ) << 8;
            match_length >>= 4;
            match_length += BREAK_EVEN;

			// This if prevents us from overrunning the output buffer if the last
			// match we find is longer than the remaining space in the output buffer.
			// This might best be fixed in the Compression routine, but it was much
			// easier to do here, so...  SCR 11/11/98
			if (match_length < size) {
				// Normal case
				size -= match_length + 1;
			} else {
				// End case
				size = 0;
				match_length = size-1;
			}

			// Write the code word into the output buffer
            for ( i = 0 ; i <= match_length ; i++ ) {
                c = ctxt.window[ MOD_WINDOW( match_position + i ) ];
                *output_string=c;
                output_string++;
                ctxt.window[ current_position ] = c;
                current_position = MOD_WINDOW( current_position + 1 );
            }
        }
    }
    input_count=input_string-inputHead;
    return( input_count);
}

#ifdef C_LINKAGE
} // extern "C"
#endif

