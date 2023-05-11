#!/bin/zsh

includePath="/opt/homebrew/opt/llvm/include/"
libPath="/opt/homebrew/opt/llvm/lib/"

clang++ -std=c++20 llvmc.cpp -I$includePath -L$libPath -lLLVM -dynamiclib -o lib/libllvmc.dylib

clang -std=c99 stack_walker.c -dynamiclib -o lib/libstack_walker.dylib