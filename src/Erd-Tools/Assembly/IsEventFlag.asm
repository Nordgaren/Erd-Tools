mov rcx,0x{0:X2}		;EventFlagMan
mov rdx,0x{1:X2}		;Event Flag ID

sub rsp,0x28
call 0x{2:X2}			;SetEventFlagFunction
mov rcx, 0x{3:X2}		;return value pointer
mov [rcx], rax
add rsp,0x28
ret