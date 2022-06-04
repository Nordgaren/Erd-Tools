sub rsp,48
mov rax,[0x{0:X2}]
mov rcx,[rax+18]
mov rdx,[rax+08]
mov eax,[0x{1:X2}]
lea r8d,[eax-000003E8]
call 0x{2:X2}
add rsp,48
ret