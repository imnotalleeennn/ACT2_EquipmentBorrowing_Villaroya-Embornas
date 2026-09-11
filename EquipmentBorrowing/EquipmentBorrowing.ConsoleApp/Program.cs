using EquipmentBorrowing.Application.Models;
using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Infrastructure.Repositories;

// Initialize repositories (Manual Dependency Injection)
var studentRepo = new InMemoryStudentRepository();
var equipmentRepo = new InMemoryEquipmentRepository();
var borrowingRepo = new InMemoryBorrowingRepository();

// Initialize application services
var borrowService = new BorrowEquipmentService(studentRepo, equipmentRepo, borrowingRepo);
var returnService = new ReturnEquipmentService(equipmentRepo, borrowingRepo);
var availableService = new GetAvailableEquipmentService(equipmentRepo);

Console.WriteLine("==================================================================");
Console.WriteLine("    CAMPUS EQUIPMENT BORROWING SYSTEM - DEMONSTRATION");
Console.WriteLine("    (Clean Architecture - Laboratory Activity 1)");
Console.WriteLine("==================================================================\n");

// --- INITIAL STATE ---
Console.WriteLine("[INITIAL STATE] Available Equipment:");
var initialAvailable = await availableService.ExecuteAsync();
foreach (var item in initialAvailable)
{
    Console.WriteLine($"  - [ID: {item.Id}] {item.Name} (Available: {item.IsAvailable})");
}
Console.WriteLine();

// --- SCENARIO 1: Successful Borrowing ---
Console.WriteLine("--- SCENARIO 1: Successful Borrow Request ---");
Console.WriteLine("Action: Student 1 (Juan Dela Cruz) borrows Equipment 1 (Epson LCD Projector)");
BorrowResult result1 = await borrowService.ExecuteAsync(studentId: 1, equipmentId: 1);
PrintBorrowResult(result1);

// --- SCENARIO 2: Ineligible Student ---
Console.WriteLine("--- SCENARIO 2: Ineligible Student Attempt ---");
Console.WriteLine("Action: Student 2 (Maria Santos, Ineligible) attempts to borrow Equipment 3 (HDMI Adapter)");
BorrowResult result2 = await borrowService.ExecuteAsync(studentId: 2, equipmentId: 3);
PrintBorrowResult(result2);

// --- SCENARIO 3: Equipment Already Borrowed / Unavailable ---
Console.WriteLine("--- SCENARIO 3: Unavailable Equipment Attempt ---");
Console.WriteLine("Action: Student 1 (Juan Dela Cruz) attempts to borrow Equipment 2 (Dell Latitude Laptop, already borrowed)");
BorrowResult result3 = await borrowService.ExecuteAsync(studentId: 1, equipmentId: 2);
PrintBorrowResult(result3);

// --- SCENARIO 4: Maximum Borrow Limit Reached ---
Console.WriteLine("--- SCENARIO 4: Maximum Borrow Limit Reached ---");
Console.WriteLine("Action: Student 3 (Pedro Penduko, Max Limit: 1) borrows Equipment 3 (HDMI Adapter)");
BorrowResult result4a = await borrowService.ExecuteAsync(studentId: 3, equipmentId: 3);
PrintBorrowResult(result4a);

Console.WriteLine("Action: Student 3 attempts to borrow Equipment 4 (Canon DSLR Camera) exceeding their limit");
BorrowResult result4b = await borrowService.ExecuteAsync(studentId: 3, equipmentId: 4);
PrintBorrowResult(result4b);

// --- SCENARIO 5: Non-Existent Entities ---
Console.WriteLine("--- SCENARIO 5: Non-Existent Entity Validation ---");
Console.WriteLine("Action: Non-existent Student (ID: 999) attempts to borrow Equipment 4");
BorrowResult result5a = await borrowService.ExecuteAsync(studentId: 999, equipmentId: 4);
PrintBorrowResult(result5a);

Console.WriteLine("Action: Student 1 attempts to borrow non-existent Equipment (ID: 888)");
BorrowResult result5b = await borrowService.ExecuteAsync(studentId: 1, equipmentId: 888);
PrintBorrowResult(result5b);

// --- SCENARIO 6: Returning Equipment ---
Console.WriteLine("--- SCENARIO 6: Return Equipment ---");
Console.WriteLine("Action: Return Equipment 2 (Dell Latitude Laptop)");
ReturnResult returnResult = await returnService.ExecuteAsync(equipmentId: 2);
Console.WriteLine($"Status  : {(returnResult.Success ? "SUCCESS" : "FAILED")}");
Console.WriteLine($"Message : {returnResult.Message}\n");

// --- SCENARIO 7: Verify Equipment Is Available Again After Return ---
Console.WriteLine("--- SCENARIO 7: Updated Available Equipment List ---");
var updatedAvailable = await availableService.ExecuteAsync();
foreach (var item in updatedAvailable)
{
    Console.WriteLine($"  - [ID: {item.Id}] {item.Name} (Available: {item.IsAvailable})");
}
Console.WriteLine();

Console.WriteLine("==================================================================");
Console.WriteLine("Demonstration completed successfully.");
Console.WriteLine("==================================================================");

if (!Console.IsInputRedirected)
{
    Console.WriteLine("\nPress any key to exit...");
    Console.ReadKey();
}

static void PrintBorrowResult(BorrowResult result)
{
    Console.WriteLine($"Status  : {(result.Success ? "SUCCESS" : "FAILED")}");
    Console.WriteLine($"Message : {result.Message}\n");
}