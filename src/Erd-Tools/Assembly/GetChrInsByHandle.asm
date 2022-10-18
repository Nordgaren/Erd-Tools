mov rcx,0x{0:X2}            ;WorldChrManPtr	
mov rdx,0x{1:X2}            ;ChrHandlePtr	
call 0x{2:X2}               ;GetChrInsByHandle
mov rcx, 0x{3:X2}
mov [rcx], rax    ;Move return to buffer		
ret