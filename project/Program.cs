Imports System
Imports MySql.Data.MySqlClient

Module Program
    Sub Main(args As String())
        Dim connectionString As String = "server=localhost;database=HospitalManagement;uid=root;pwd=Leonard123;"

        ' Create tables if they don't exist
        CreateTablesIfNotExists(connectionString)

        Dim exitProgram As Boolean = False

        While Not exitProgram
            Console.WriteLine("----------------------------------------------")
            Console.WriteLine("Medication Management System")
            Console.WriteLine("1. Enter New Medicine")
            Console.WriteLine("2. View Current Stock")
            Console.WriteLine("3. Place Order")
            Console.WriteLine("4. Delete Medicine")
            Console.WriteLine("5. Exit")

            Console.WriteLine("Enter your choice: ")
            Dim choice As Integer = Convert.ToInt32(Console.ReadLine())
            Select Case choice
                Case 1
                    EnterNewMedicine(connectionString)
                Case 2
                    ViewCurrentStock(connectionString)
                Case 3
                    PlaceOrder(connectionString)
                Case 4
                    DeleteMedicine(connectionString)
                Case 5
                    exitProgram = True
                Case Else
                    Console.WriteLine("Invalid choice. Please enter a number between 1 and 5.")
            End Select
        End While
    End Sub

    Sub CreateTablesIfNotExists(connectionString As String)
        Using connection As New MySqlConnection(connectionString)
            connection.Open()

            ' SQL commands to create tables
            Dim createTablesSql As String =
                "CREATE TABLE IF NOT EXISTS types_of_medicine
                (id INT AUTO_INCREMENT PRIMARY KEY,name VARCHAR(255) NOT NULL,  

                dosage VARCHAR(50) NOT NULL);CREATE TABLE IF NOT EXISTS current_stock
                (id INT AUTO_INCREMENT PRIMARY KEY,medicine_id INT, serial_number VARCHAR(100) NOT NULL,expiration_date DATE NOT NULL, FOREIGN KEY (medicine_id) REFERENCES types_of_medicine(id)); CREATE TABLE IF NOT EXISTS orders_to_be_placed ( id INT AUTO_INCREMENT PRIMARY KEY, medicine_id INT, quantity INT NOT NULL, FOREIGN KEY (medicine_id) REFERENCES types_of_medicine(id) ); CREATE TABLE IF NOT EXISTS doctor_and_nurse ( id INT AUTO_INCREMENT PRIMARY KEY, name VARCHAR(100) NOT NULL, surname VARCHAR(100) NOT NULL, type ENUM('Doctor', 'Nurse') NOT NULL ); CREATE TABLE IF NOT EXISTS patients ( id INT AUTO_INCREMENT PRIMARY KEY, name VARCHAR(100) NOT NULL, surname VARCHAR(100) NOT NULL, date_of_entry DATE NOT NULL );CREATE TABLE IF NOT EXISTS medicines (id INT AUTO_INCREMENT PRIMARY KEY, name VARCHAR(255) NOT NULL, dosage VARCHAR(50) NOT NULL, quantity INT NOT NULL); CREATE TABLE IF NOT EXISTS orders (id INT AUTO_INCREMENT PRIMARY KEY, medicine_id INT NOT NULL, quantity INT NOT NULL, doctor_name VARCHAR(100) NOT NULL)"

            Dim command As New MySqlCommand(createTablesSql, connection)
            command.ExecuteNonQuery()
        End Using
    End Sub

    Sub EnterNewMedicine(connectionString As String)
        Console.WriteLine("Enter details for the new medicine:")
        Console.WriteLine("Medicine Name: ")
        Dim medicineName As String = Console.ReadLine()
        Console.WriteLine("Medicine Dosage: ")
        Dim medicineDosage As String = Console.ReadLine()
        Console.WriteLine("Quantity: ")
        Dim quantity As Integer = Convert.ToInt32(Console.ReadLine())

        Using connection As New MySqlConnection(connectionString)
            connection.Open()

            Dim insertMedicationQuery As String = "INSERT INTO medicines (name, dosage, quantity) VALUES (@name, @dosage, @quantity)"
            Dim insertMedicationCommand As New MySqlCommand(insertMedicationQuery, connection)

            insertMedicationCommand.Parameters.AddWithValue("@name", medicineName)
            insertMedicationCommand.Parameters.AddWithValue("@dosage", medicineDosage)
            insertMedicationCommand.Parameters.AddWithValue("@quantity", quantity)

            insertMedicationCommand.ExecuteNonQuery()

            Console.WriteLine("Medicine added successfully.")
        End Using
    End Sub

    Sub ViewCurrentStock(connectionString As String)
        Using connection As New MySqlConnection(connectionString)
            connection.Open()

            Dim viewStockQuery As String = "SELECT * FROM medicines"
            Dim viewStockCommand As New MySqlCommand(viewStockQuery, connection)

            Dim reader As MySqlDataReader = viewStockCommand.ExecuteReader()

            Console.WriteLine("Current Stock:")
            Console.WriteLine("ID | Medicine Name | Dosage | Quantity")
            While reader.Read()
                Console.WriteLine($"{reader("id")} | {reader("name")} | {reader("dosage")} | {reader("quantity")}")
            End While
            reader.Close()
        End Using
    End Sub

    Sub PlaceOrder(connectionString As String)
        Console.WriteLine("Enter your name: ")
        Dim doctorName As String = Console.ReadLine()
        Console.WriteLine("Enter the ID of the medicine you want to order: ")
        Dim medicineId As Integer = Convert.ToInt32(Console.ReadLine())
        Console.WriteLine("Enter quantity: ")
        Dim quantity As Integer = Convert.ToInt32(Console.ReadLine())

        Using connection As New MySqlConnection(connectionString)
            connection.Open()

            Dim placeOrderQuery As String = "INSERT INTO orders (medicine_id, quantity, doctor_name) VALUES (@medicine_id, @quantity, @doctor_name)"
            Dim placeOrderCommand As New MySqlCommand(placeOrderQuery, connection)

            placeOrderCommand.Parameters.AddWithValue("@medicine_id", medicineId)
            placeOrderCommand.Parameters.AddWithValue("@quantity", quantity)
            placeOrderCommand.Parameters.AddWithValue("@doctor_name", doctorName)

            placeOrderCommand.ExecuteNonQuery()

            Console.WriteLine("Order placed successfully.")
        End Using
    End Sub

    Sub DeleteMedicine(connectionString As String)
        Console.WriteLine("Enter the ID of the medicine you want to delete: ")
        Dim medicineId As Integer = Convert.ToInt32(Console.ReadLine())

        Using connection As New MySqlConnection(connectionString)
            connection.Open()

            Dim deleteMedicineQuery As String = "DELETE FROM medicines WHERE id = @id"
            Dim deleteMedicineCommand As New MySqlCommand(deleteMedicineQuery, connection)

            deleteMedicineCommand.Parameters.AddWithValue("@id", medicineId)

            Dim rowsAffected As Integer = deleteMedicineCommand.ExecuteNonQuery()

            If rowsAffected > 0 Then
                Console.WriteLine("Medicine deleted successfully.")
            Else
                Console.WriteLine("No medicine found with the specified ID.")
            End If
        End Using
    End Sub

End Module