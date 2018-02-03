---------------------------------------------
DEFINITIONS:
---------------------------------------------

---------------------------------------------
FRAG: A frag is a list of one-channel audio files (each identified by their TLKIDs), each representing a different 
      speaker or voice saying the same word or phrase.  This allows the AI to have different speaking voices. 
      There can be up to 14 voices (numbered 0 - 13) in a single frag.  

EVAL: A set of related, numbered frags from which the AI communications will automatically select the appropriate "frag" to play at run-time.  
      For example, one of the built-in evals is a list of runway numbers from zero to 360.  

COMM: An ordered list of frags and/or evals that defines a complete phrase, sentence, or message that can be spoken by the AI comms.  

---------------------------------------------


---------------------------------------------
FILES:
---------------------------------------------

FRAGFILE.BIN
  - a binary file containing the list of "frag" definitions

EVALFILE.BIN 
  - a binary file containing the list of "eval" definitions

COMMFILE.BIN
  - a binary file containing the list of "comm" definitions

FALCON.TLK
  - a binary file containing a set of individually-numbered, compressed audio files that are referenced by the frag definitions and the eval definitions

---------------------------------------------


---------------------------------------------
TLKTOOL
---------------------------------------------

TLKTOOL is a command-line utility which allows for editing Falcon's FRAGFILE.BIN, EVALFILE.BIN, COMMFILE.BIN, and FALCON.TLK file.  

TLKTOOL can convert Falcon's FRAGFILE.BIN file, EVALFILE.BIN file, and COMMFILE.BIN file to plaintext human-readable human-editable XML files, and can 
also convert those XML files back to the binary format that Falcon expects.  This allows for editing the AI comms definitions using nothing more than
an ordinary text editor like Notepad.

TLKTOOL can also export all of the audio files in Falcon's FALCON.TLK file to a set of ordinary .WAV audio files, which can then be edited or replaced in bulk.  
TLKTOOL can build up a fresh new FALCON.TLK file from a set of .WAV files (or from a set of compressed .WAV equivalents such as a set of .LH files or .SPX files).  
It further supports converting .WAV files to and from various proprietary and/or open compression formats including Lernout & Hauspie StreamTalk 8.0 (used by legacy Falcon versions)
as well as the new, open-source Speex compression format used in later BMS flavors of Falcon.

You can also use TLKTOOL make single-file edits to specific audio files stored within the FALCON.TLK by importing replacement audio files into the FALCON.TLK file.



TLKTOOL supports the following actions:

     .TLK file exporting & importing:

    TLK2WAV:  exports a set of .WAV files from the .TLK file (requires specifying the underlying compression format used inside the .TLK file)
    TLK2LH:   exports a set of LH-compressed audio files from the .TLK file (assuming the .TLK file contains .LH-format audio files)
    TLK2SPX:  exports SPX-compressed audio files from the .TLK file   (assuming the .TLK file contains .SPX-format audio files)
    WAV2TLK:  imports .WAV files into a .TLK file (or builds a new .TLK file from a set of .WAV files) (requires specifying the compression format used inside the .TLK file)
    LH2TLK:   imports LH-compressed audio files into a LH-format .TLK file (or builds a new .TLK file from a set of .LH files)
    SPX2TLK:  imports SPX-compressed audio files into a SPX-format .TLK file (or builds a new .TLK file from a set of .SPX files)


      COMMFILE.BIN, FRAGFILE.BIN, and EVALFILE.BIN exporting:

    COMM2XML:  exports the contents of a commFile.bin file to an XML file
    FRAG2XML:  exports the contents of a fragFile.bin file to an XML file
    EVAL2XML:  exports the contents of a evalFile.bin file to an XML file
    XML2COMM:  builds a commFile.bin file from an XML file
    XML2FRAG:  builds a fragFile.bin file from an XML file
    XML2EVAL:  builds a evalFile.bin file from an XML file


      .WAV file compression, decompression, and format conversion (conversion of .WAV files to and from .LH or .SPX compressed files)

    WAV2SPX: converts a .WAV file (or a folder of .WAV files) to .SPX file(s)
    WAV2LH: converts a .WAV file (or a folder of .WAV files) to .LH file(s)
    SPX2WAV: converts a .SPX file (or a folder of .SPX files) to .WAV file(s)
    LH2WAV: converts a .LH file (or a folder of .LH files) to .WAV file(s)
    LH2SPX converts a .LH file (or a folder of .LH files) to .SPX file(s)
    SPX2LH converts a .SPX file (or a folder of .SPX files) to .LH file(s)


-------------------------------------------------------------------------------


