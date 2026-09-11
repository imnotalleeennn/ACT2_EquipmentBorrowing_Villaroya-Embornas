# Campus Equipment Borrowing System

A layered desktop application built following **Clean / Onion Architecture** principles and the **Model-View-ViewModel (MVVM)** pattern in .NET 10 using **Avalonia UI** and **CommunityToolkit.Mvvm**.

This project implements:
- **Laboratory Activity 1**: From Requirements to Application Structure
- **Laboratory Activity 2**: Extending the Application with Avalonia UI and MVVM

---

## Part A: System Analysis (Laboratory Activity 1)

### 1. Actors

- **Student**: An authorized learner who requests to borrow equipment, views currently available equipment, and returns borrowed items.
  - *Expectation*: The student expects to quickly see what items are available, successfully borrow eligible equipment within limits, and return items when finished.
- **Lab Administrator / System**: The authority that establishes lab rules, validates eligibility and borrow limits, tracks active borrowings, and updates equipment inventory availability.
  - *Expectation*: The system expects that all borrowing rules are strictly enforced (eligibility, availability, quotas) and that equipment state is reliably tracked without data corruption.

---

### 2. Major Use Cases

#### Use Case 1: Borrow Equipment (Primary Focus)

| Item | Description |
|---|---|
| **Use Case** | Borrow Equipment |
| **Primary Actor** | Student |
| **Preconditions** | 1. The student is registered in the system.<br>2. The equipment item exists in the lab inventory. |
| **Main Action** | 1. The student submits a request to borrow a specific piece of equipment.<br>2. The system verifies the student exists and is eligible to borrow.<br>3. The system verifies the equipment exists and is currently available.<br>4. The system checks that the student has not exceeded their active borrow limit.<br>5. The system creates an active borrowing record with a due date.<br>6. The system marks the equipment as unavailable.<br>7. The system confirms the approved borrow request to the student. |
| **Expected Result** | A new active borrowing record is created, the equipment availability becomes `false`, and a success confirmation with due date is returned. |
| **Possible Failure** | - Student does not exist.<br>- Student is not eligible to borrow (account blocked or on hold).<br>- Equipment does not exist.<br>- Equipment is currently unavailable / already borrowed.<br>- Student has already reached their maximum borrow quota. |

#### Use Case 2: Return Equipment

| Item | Description |
|---|---|
| **Use Case** | Return Equipment |
| **Primary Actor** | Student |
| **Preconditions** | The equipment was previously borrowed and currently has an active borrowing record. |
| **Main Action** | 1. The student brings the equipment back to the lab.<br>2. The system looks up the active borrowing record for the item.<br>3. The system records the return date and marks the borrowing status as `Returned`.<br>4. The system updates the equipment status to available (`IsAvailable = true`).<br>5. The system confirms the return. |
| **Expected Result** | Borrowing status is updated to `Returned`, return timestamp is captured, and the equipment availability is restored to `true`. |
| **Possible Failure** | - Equipment does not exist.<br>- No active borrowing record exists for the equipment (e.g., item was already returned). |

#### Use Case 3: Search / View Available Equipment

| Item | Description |
|---|---|
| **Use Case** | Search / View Available Equipment |
| **Primary Actor** | Student |
| **Preconditions** | Lab inventory is accessible. |
| **Main Action** | 1. The student requests a list of equipment currently available for borrowing.<br>2. The system queries inventory for items where `IsAvailable == true`.<br>3. The system displays the list of available equipment. |
| **Expected Result** | The student receives an accurate, up-to-date catalog of all equipment ready to be borrowed. |
| **Possible Failure** | Inventory is empty or no equipment matches availability criteria. |

---

### 3. Domain Concepts

#### Concept 1: Student
1. **Information Contained**:
   - `Id`: Unique numeric identifier.
   - `Name`: Full name of the student.
   - `IsEligibleToBorrow`: Boolean flag indicating if the student is permitted to borrow.
   - `MaxBorrowLimit`: Maximum number of simultaneous active borrowings allowed.
2. **Rules / State Belonging to It**:
   - Whether the student is allowed to borrow (`IsEligibleToBorrow`).
   - Quota validation (`CanBorrow(activeCount)` checks eligibility and `activeCount < MaxBorrowLimit`).
3. **What Should NOT Be the Responsibility of this Object**:
   - Querying the database or persistence store for borrowing records.
   - Rendering UI elements or printing formatted error dialogues.
   - Directly mutating equipment availability status.

#### Concept 2: Equipment
1. **Information Contained**:
   - `Id`: Unique numeric identifier.
   - `Name`: Descriptive name of the equipment (e.g., "Epson LCD Projector").
   - `IsAvailable`: Boolean indicating whether the equipment is currently in the lab and available for loan.
2. **Rules / State Belonging to It**:
   - State transition when borrowed (`MarkAsBorrowed() -> IsAvailable = false`).
   - State transition when returned (`MarkAsReturned() -> IsAvailable = true`).
3. **What Should NOT Be the Responsibility of this Object**:
   - Storing student loan history.
   - Checking whether a student has overdue fines.
   - Directly saving itself to a database or filesystem.

#### Concept 3: Borrowing
1. **Information Contained**:
   - `Id`: Unique identifier for the borrowing transaction.
   - `StudentId`: Reference to the borrowing student.
   - `EquipmentId`: Reference to the borrowed item.
   - `BorrowDate`: Date and time when the borrowing occurred.
   - `DueDate`: Date by which the item must be returned.
   - `ReturnDate`: Optional timestamp (`DateTime?`) set when the item is returned.
   - `Status`: Current state (`BorrowingStatus.Active` or `BorrowingStatus.Returned`).
2. **Rules / State Belonging to It**:
   - Managing status transition upon return (`MarkAsReturned(returnDate)`).
   - Enforcing active status upon initial creation.
3. **What Should NOT Be the Responsibility of this Object**:
   - Formatting console or UI error messages.
   - Deciding how or where borrowing records are stored.
   - Validating student eligibility or equipment existence.

---

## Part I: Architecture Explanation (Laboratory Activity 1)

### 1. Solution Structure

The solution strictly adheres to Clean / Onion Architecture:

- **`EquipmentBorrowing.Domain`**:
  The innermost layer containing core enterprise entities and business rules (`Student`, `Equipment`, `Borrowing`, `BorrowingStatus`). It has zero external dependencies and does not depend on any framework or persistence library.
- **`EquipmentBorrowing.Application`**:
  Contains the application use cases and business workflow orchestrations (`BorrowEquipmentService`, `ReturnEquipmentService`, `GetAvailableEquipmentService`, `GetAllEquipmentService`, `GetAllStudentsService`, `GetActiveBorrowingsService`). It defines repository interfaces (`IStudentRepository`, `IEquipmentRepository`, `IBorrowingRepository`) and result DTOs (`BorrowResult`, `ReturnResult`, `ActiveBorrowingDto`). It depends only on `Domain`.
- **`EquipmentBorrowing.Infrastructure`**:
  Contains data access implementations and external concerns. Implements the repository interfaces defined in `Application` using in-memory data structures (`InMemoryStudentRepository`, `InMemoryEquipmentRepository`, `InMemoryBorrowingRepository`). It depends on `Application` and `Domain`.
- **`EquipmentBorrowing.ConsoleApp`**:
  The initial presentation / composition root for Activity 1. Demonstrates use cases via formatted console output.
- **`EquipmentBorrowing.Desktop`**:
  The Avalonia MVVM desktop user interface introduced in Laboratory Activity 2.
- **`EquipmentBorrowing.Tests`**:
  Contains automated unit tests (using xUnit) that verify business rules, limit checks, error conditions, and availability transitions without needing a database.

---

### 2. Dependency Direction

Dependencies flow strictly **inward** toward the domain, upholding the **Dependency Inversion Principle (DIP)**:

```
+-------------------------------+       +-------------------------------+
| EquipmentBorrowing.ConsoleApp |       |  EquipmentBorrowing.Desktop   |
| (Activity 1 CLI Presentation) |       |   (Activity 2 Avalonia UI)    |
+-------------------------------+       +-------------------------------+
                |                                       |
                +-------------------+-------------------+
                                    |
                                    v
+------------------------------------+   +----------------------------+
| EquipmentBorrowing.Infrastructure  |   | EquipmentBorrowing.Tests   |
| (In-Memory Repositories / Storage) |   | (Automated xUnit Tests)    |
+------------------------------------+   +----------------------------+
       |                                              |
       +----------------------+-----------------------+
                              |
                              v
             +----------------------------------+
             |  EquipmentBorrowing.Application  |
             |  (Use Cases, Repositories Intf)  |
             +----------------------------------+
                              |
                              v
             +----------------------------------+
             |     EquipmentBorrowing.Domain    |
             |   (Core Entities & Enums Only)   |
             +----------------------------------+
```

---

### 3. Use Case Mapping

#### Implemented Use Case: **Borrow Equipment**

- **Actor**: Student
- **Use Case**: Borrow Equipment
- **Application Service**: `BorrowEquipmentService`
- **Domain Objects Used**:
  - `Student` (holds eligibility and borrowing quota)
  - `Equipment` (holds availability state and `MarkAsBorrowed()`)
  - `Borrowing` (records the loan transaction)
  - `BorrowingStatus` (enum `Active`)
- **Repository Interfaces Used**:
  - `IStudentRepository` (`GetByIdAsync`, `GetAllAsync`)
  - `IEquipmentRepository` (`GetByIdAsync`, `UpdateAsync`, `GetAllAsync`)
  - `IBorrowingRepository` (`GetActiveBorrowingCountAsync`, `AddAsync`)
- **Infrastructure Implementations Used**:
  - `InMemoryStudentRepository`
  - `InMemoryEquipmentRepository`
  - `InMemoryBorrowingRepository`

---

### 4. Activity 1 Reflection Questions & Answers

#### 1. Why should the application service depend on a repository interface instead of directly depending on a database implementation?
> **Answer**:
> Depending on an abstraction (an interface) decouples the core business logic from specific database technologies (such as SQLite, PostgreSQL, or EF Core). This satisfies the **Dependency Inversion Principle** (high-level modules should not depend on low-level modules; both should depend on abstractions). It allows the data storage mechanism to be swapped or upgraded without changing a single line of business logic, and it makes the application service easily unit-testable using mock or in-memory repositories without requiring an active database server.

#### 2. Which parts of your current solution could remain unchanged if SQLite were added later?
> **Answer**:
> - **`EquipmentBorrowing.Domain`**: Remains 100% unchanged because domain concepts do not care how data is stored.
> - **`EquipmentBorrowing.Application`**: Remains 100% unchanged. The services (`BorrowEquipmentService`, `ReturnEquipmentService`, `GetAvailableEquipmentService`), result models, and repository interfaces (`IStudentRepository`, etc.) remain identical.
> - **`EquipmentBorrowing.Desktop` Views & ViewModels**: Remain unchanged because ViewModels depend on Application Services, not repositories.
> - **`EquipmentBorrowing.Tests`**: Unit tests verifying application logic remain valid and unchanged.
> - **Only `EquipmentBorrowing.Infrastructure`** would be expanded with SQLite repository implementations, and the DI registration in `App.axaml.cs` would bind the interfaces to SQLite repositories.

#### 3. Which project would eventually contain Avalonia Views?
> **Answer**:
> The **`EquipmentBorrowing.Desktop`** project contains the Avalonia AXAML Views (`MainWindow.axaml`, `EquipmentView.axaml`, `BorrowingsView.axaml`) and ViewModels (`MainWindowViewModel`, `EquipmentViewModel`, `BorrowingsViewModel`) following the MVVM pattern.

#### 4. Should an Avalonia button directly execute database queries? Why or why not?
> **Answer**:
> **No, an Avalonia button should never directly execute database queries.** Doing so would cause severe architectural issues:
> 1. **Tight Coupling & Violation of Separation of Concerns**: Directly binds the UI layout to database schema and SQL queries.
> 2. **Bypassing Business Rules**: All validation rules (checking student eligibility, borrowing limits, availability) would be bypassed or duplicated in the UI.
> 3. **UI Thread Blocking**: Long-running database operations on the UI thread freeze the application interface.
> Instead, buttons bind to `RelayCommand`s on ViewModels, which call Application Services asynchronously.

#### 5. What part of your implementation represents the actual business operation requested by the actor?
> **Answer**:
> The **`BorrowEquipmentService.ExecuteAsync()`** method in the `EquipmentBorrowing.Application` project represents the actual business operation. It coordinates student eligibility verification, equipment availability checks, borrow limit enforcement, `Borrowing` record creation, equipment state mutation, and repository persistence.

---

## Part L: Laboratory Activity 2 – Avalonia Desktop UI and MVVM

### 1. Desktop Project Responsibility
The **`EquipmentBorrowing.Desktop`** project serves as the graphical presentation layer of the application. Its core responsibilities include:
- Displaying data to the user using XAML controls, responsive layouts, and data templates.
- Collecting and validating user presentation input (e.g. dropdown selections, date picking).
- Managing presentation state (selected items, active views, loading state, user feedback banners).
- Invoking application use cases via `CommunityToolkit.Mvvm` relay commands.
- Serving as the **Central Composition Root** in `App.axaml.cs`, configuring Dependency Injection with `Microsoft.Extensions.DependencyInjection`.

It interacts with other projects by referencing:
- `EquipmentBorrowing.Application`: Calls application services and receives DTOs / Result models.
- `EquipmentBorrowing.Infrastructure`: Registers in-memory repository singleton implementations in the DI container.
- Note: `EquipmentBorrowing.Domain` and `EquipmentBorrowing.Application` have **zero references** to Avalonia or Desktop.

---

### 2. Updated Architecture Diagram

```
+-----------------------------------------------------------------------------------+
|                            EquipmentBorrowing.Desktop                             |
|                                                                                   |
|   Avalonia Views:           MainWindow.axaml   EquipmentView.axaml  BorrowingsView|
|                                    │                  │                 │         |
|                          Data Binding / Commands (XAML)                           |
|                                    ▼                  ▼                 ▼         |
|   ViewModels:               MainWindowVM       EquipmentVM       BorrowingsVM     |
|   (CommunityToolkit.Mvvm)          │                  │                 │         |
+------------------------------------┼──────────────────┼─────────────────┼---------+
                                     │ Invokes Use Case │                 │
                                     ▼                  ▼                 ▼
+-----------------------------------------------------------------------------------+
|                           EquipmentBorrowing.Application                          |
|                                                                                   |
|   Application Services:     BorrowEquipmentService    ReturnEquipmentService      |
|                             GetAllEquipmentService    GetActiveBorrowingsService  |
|                                    │                          │                   |
|                        Orchestrates Domain & Repos            │                   |
|                                    ▼                          ▼                   |
|   Repository Interfaces:    IEquipmentRepository    IBorrowingRepository          |
|                             IStudentRepository                                    |
+-----------------------------------------------------------------------------------+
       ▲                                                          │
       │ Implements Abstraction                                   │ References
       │                                                          ▼
+------------------------------------+             +----------------------------+
| EquipmentBorrowing.Infrastructure  |             |  EquipmentBorrowing.Domain |
|                                    |             |                            |
| InMemoryEquipmentRepository        |             | Equipment, Student,        |
| InMemoryBorrowingRepository        |             | Borrowing, BorrowingStatus |
| InMemoryStudentRepository          |             |                            |
+------------------------------------+             +----------------------------+
```

---

### 3. Borrow Equipment Flow

When the user requests to borrow equipment, the complete flow proceeds as follows:

```
[User Action]
     │
     │ 1. User selects Student from ComboBox, clicks Equipment item, and clicks [ Confirm & Borrow ]
     ▼
[EquipmentView.axaml]
     │
     │ 2. Command binding triggers {Binding BorrowCommand}
     ▼
[EquipmentViewModel.BorrowAsync()]
     │
     │ 3. Presentation Validation: Checks if Student and Equipment are selected.
     │    If not, sets StatusMessage = "Please select..." and aborts.
     │
     │ 4. Invokes application service:
     │    await _borrowEquipmentService.ExecuteAsync(studentId, equipmentId);
     ▼
[BorrowEquipmentService.ExecuteAsync()]
     │
     │ 5. Business Validation:
     │    - Student exists and is eligible?
     │    - Equipment exists and is available?
     │    - Student active borrowing count < MaxBorrowLimit?
     │
     │ 6. Creates new Borrowing record.
     │    Calls equipment.MarkAsBorrowed().
     │    Calls _borrowingRepo.AddAsync(borrowing).
     │    Calls _equipmentRepo.UpdateAsync(equipment).
     │
     │ 7. Returns BorrowResult (Success: true, Message: "Successfully borrowed...")
     ▼
[EquipmentViewModel]
     │
     │ 8. Receives BorrowResult. Updates StatusMessage, sets IsSuccess, and refreshes catalog.
     ▼
[EquipmentView.axaml]
     │
     │ 9. UI updates via data binding: Status banner turns green, item badge changes to unavailable.
```

---

### 4. Return Equipment Flow

When the user returns borrowed equipment:

```
[User Action]
     │
     │ 1. User selects a loan from Active Borrowings and clicks [ Process Return ]
     ▼
[BorrowingsView.axaml]
     │
     │ 2. Command binding triggers {Binding ReturnCommand}
     ▼
[BorrowingsViewModel.ReturnAsync()]
     │
     │ 3. Presentation Validation: Checks if SelectedBorrowing is not null.
     │    Invokes application service:
     │    await _returnEquipmentService.ExecuteAsync(equipmentId);
     ▼
[ReturnEquipmentService.ExecuteAsync()]
     │
     │ 4. Business Validation & State Change:
     │    - Locates active borrowing record via _borrowingRepo.GetActiveBorrowingByEquipmentIdAsync().
     │    - Calls borrowing.MarkAsReturned(DateTime.UtcNow).
     │    - Calls equipment.MarkAsReturned() (IsAvailable = true).
     │    - Persists updates via _borrowingRepo.UpdateAsync() and _equipmentRepo.UpdateAsync().
     │    - Returns ReturnResult.Ok("Successfully returned...").
     ▼
[BorrowingsViewModel]
     │
     │ 5. Receives ReturnResult. Refreshes ActiveBorrowings observable collection.
     ▼
[BorrowingsView.axaml]
     │
     │ 6. Returned loan disappears from active list; status banner displays success confirmation.
```

---

### 5. Activity 2 Reflection Questions & Answers

#### 1. Why should the View not call a repository directly?
> **Answer**:
> The View represents pure user interface presentation and layout. Calling a repository directly from the View couples the UI to data-access technology, circumvents all application business rules (such as borrow limits and eligibility checks), makes automated UI testing impossible, and violates the **Single Responsibility Principle** and **Separation of Concerns**.

#### 2. Why should business rules not be implemented in the ViewModel?
> **Answer**:
> ViewModels belong to the presentation layer; their responsibility is to manage presentation state, user inputs, and view-specific commands. Placing business rules (such as checking if a student is eligible or enforcing maximum borrowing limits) inside ViewModels causes duplicate business logic across different UIs (e.g. Console App vs. Desktop App vs. Web App), leads to inconsistencies, and breaks the independent testability of the core domain.

#### 3. What is the responsibility of the ViewModel?
> **Answer**:
> The ViewModel acts as the bridge between the View and the Application layer:
> - Maintains observable presentation state (e.g., `EquipmentList`, `SelectedStudent`, `StatusMessage`).
> - Exposes relay commands (`BorrowCommand`, `ReturnCommand`, `NavigateCommand`) triggered by user interactions.
> - Performs presentation-level validation (ensuring mandatory inputs/selections are provided).
> - Calls application services to perform operations and translates the results into user-facing feedback.

#### 4. Why can the existing Application layer work without knowing that Avalonia is being used?
> **Answer**:
> Because of the **Inward Dependency Rule** of Clean Architecture. The Application layer defines pure C# interfaces, services, and models that have zero dependencies on any UI framework or GUI library. It receives standard primitive arguments or domain DTOs and returns domain results. It does not know or care whether the caller is Avalonia UI, WPF, a console application, or a web API.

#### 5. What advantage is gained from registering dependencies in one composition point?
> **Answer**:
> A centralized composition root (located in `App.axaml.cs`):
> - Provides a single location to manage component lifetimes (e.g., registering repositories as **Singletons** so in-memory inventory persists across view switching).
> - Facilitates effortless swapping of implementations (e.g., replacing in-memory repositories with SQLite or PostgreSQL without touching ViewModels or Views).
> - Eliminates tightly-coupled `new` instantiations inside consuming classes, upholding the **Dependency Inversion Principle**.

#### 6. If the in-memory repository were replaced by SQLite later, which parts of the current interface should remain largely unchanged?
> **Answer**:
> The **entire user interface remains 100% unchanged**.
> - All XAML Views (`MainWindow.axaml`, `EquipmentView.axaml`, `BorrowingsView.axaml`) remain unchanged.
> - All ViewModels (`MainWindowViewModel`, `EquipmentViewModel`, `BorrowingsViewModel`) remain unchanged.
> - All Application Services and Domain models remain unchanged.
> The only change required would be creating SQLite implementations of `IEquipmentRepository`, `IStudentRepository`, and `IBorrowingRepository` in `EquipmentBorrowing.Infrastructure`, and pointing the DI registrations in `App.axaml.cs` to the new SQLite classes.

---

## Running the Projects & Automated Tests

### 1. Run the Avalonia Desktop UI (Activity 2)
```powershell
dotnet run --project EquipmentBorrowing.Desktop
```
- Interactive sidebar navigation between **Equipment Catalog** and **Active Borrowings**.
- Full borrow workflow with student dropdown, expected return date, and live validation.
- Full return workflow with active loan monitoring and inventory state restoration.

### 2. Run the Console Demonstration (Activity 1)
```powershell
dotnet run --project EquipmentBorrowing.ConsoleApp
```
Executes all 7 demonstration scenarios with detailed output.

### 3. Run Automated Unit Tests
```powershell
dotnet test EquipmentBorrowing.sln
```
All 9 unit tests pass with zero errors.