#include <stddef.h>
#include <stdint.h>
#include <limits.h>
#include <float.h>

struct i1
{
    uint8_t value: 1;
};

struct i128
{
    uint64_t first, second;
};

static unsigned int ilog2(unsigned int val) {
    if (val == 0) return UINT_MAX;
    if (val == 1) return 0;
    unsigned int ret = 0;
    while (val > 1) {
        val >>= 1;
        ret++;
    }
    return ret;
}

typedef void (*function_ptr)(void);

size_t get_function_pointer_size(){
    return 8 * sizeof(function_ptr);
}

size_t get_pointer_size(){
    return 8 * sizeof(void*);
}

size_t get_alignment_of_pointer(){
    return 8 * _Alignof(void*);
}

size_t get_alignment_of_function_pointer(){
    return 8 * _Alignof(function_ptr);
}

size_t get_alignment_of_integer(uint32_t width){
    switch(width)
    {
        case 1:
            return _Alignof(struct i1);
        case 8:
            return _Alignof(uint8_t);
        case 16:
            return _Alignof(uint16_t);
        case 32:
            return _Alignof(uint32_t);
        case 64:
            return _Alignof(uint64_t);
        case 128:
            return _Alignof(struct i128);
        
    }
}

size_t get_alignment_of_fp(uint32_t width){
    switch(width)
    {
        case 1:
            return _Alignof(long double);
        case 16:
            return _Alignof(__fp16);
        case 32:
            return _Alignof(float);
        case 64:
            return _Alignof(double);
        
    }
}

size_t get_long_double_size(){
    return LDBL_MANT_DIG + ilog2(LDBL_MAX_EXP) + 1u;
}