sub rsp,48
mov ecx,33
mov rdx,0x{0:X2}    ;eldenring.exe+3C41EC8 ?AVhkLifoAllocator + 0x8 SCAN
call TlsSetValue

mov rcx,[0x{1:X2}]
mov rcx,[rcx+0x190]
mov rcx,[rcx+0x28]
lea rcx,[rcx+0x17B0]
lea rdx,[0x{2:X2}]     ;animation pointer
mov r8d,[0x{3:X2}+0x4] ;IsLoop?

call 0x{4:X2}       ;"eldenring.exe"+4140B0
add rsp,48
ret