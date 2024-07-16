sub rsp, 0x20
mov r8,1		    ;unk
mov rdx,0x{0:X2}	;InventorySlot
mov rcx,0x{1:X2}	;EquipInventoryData
call 0x{2:X2}       ;RemoveItem
add rsp, 0x20
ret
