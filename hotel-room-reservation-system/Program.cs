//Created by Daniel Pilich
using System.Text;

Hotel hotel = new Hotel(Menu.InsertBalance());
Menu menu = new Menu(hotel);

do
{
    menu.MakeChoice();
} while (!menu.StopMenu());

class Menu
{
    private bool ending = false;
    private Hotel hotel;

    public Menu(Hotel hotel)
    {
        this.hotel = hotel;
    }

    private static void ShowMenu()
    {
        Console.Clear();

        Console.WriteLine("Hotel - Menu główne\n\t" +
            "--Operacje na pokojach--\n\t" +
            "1. Zarezerwuj pokój\n\t" +
            "2. Zarezerwuj kilka pokoi\n\t" +
            "3. Anuluj rezerwacje pokoju\n\t" +
            "4. Anuluj rezerwacje kilku pokoi\n\t" +
            "5. Sprawdź stan pokoju i jego cenę\n\t" +
            "6. Sprawdź ile jest wolnych pokoi\n\n\t" +

            "------Plan hotelu-------\n\t" +
            "7. Pokaż plan hotelu\n\n\t" +

            "-----Konto hotelowe-----\n\t" +
            "8. Sprawdź swoje saldo\n\t" +
            "9. Dodaj pieniądze na konto\n\n\t" +

            "------Testowanie--------\n\t" +
            "10. RunTest\n\n\t" +

            "0. Wciśnij 0, aby zakończyć działanie programu\n\n" +
            "Podaje cyfrę, aby wybrać opcję:");
    }

    public static int InsertOption(int max = 10)
    {
        int choice;
        do
        {
            try
            {
                choice = Convert.ToInt32(Console.ReadLine());
                if (choice < 0 || choice > max)
                {
                    Console.WriteLine("\nPodano zla liczbe!!! Sprobuj jeszcze raz.");
                    continue;
                }
                break;
            }
            catch
            {
                Console.WriteLine("\nPodano zla wartosc!!! Sprobuj jeszcze raz.");
                continue;
            }
        } while (true);

        return choice;
    }

    public static int InsertBalance()
    {
        int balance = -1;
        do
        {
            try
            {
                Console.WriteLine("Podaj ile masz pieniędzy: ");
                balance = Convert.ToInt32(Console.ReadLine());
            }
            catch
            {
                Console.WriteLine("\nNie podano liczby!!! Sprobuj jeszcze raz.");
            }
        } while (balance < 0);

        return balance;
    }

    public void MakeChoice()
    {
        ShowMenu();

        int choice = InsertOption();
        Console.WriteLine();
        switch (choice)
        {
            case 1:
                hotel.MakeReservation();
                break;
            case 2:
                hotel.MakeReservations();
                break;
            case 3:
                hotel.CancelReservation();
                break;
            case 4:
                hotel.CancelReservations();
                break;
            case 5:
                hotel.CheckARoom();
                break;
            case 6:
                Console.WriteLine("Aktualnie wolnych pokoi jest: " + hotel.TotalFreeRooms());
                break;

            case 7:
                Console.Write(hotel);
                break;

            case 8:
                Console.Write($"Twoje saldo wynosi: {hotel.Balance}");
                break;
            case 9:
                hotel.Balance += InsertBalance();
                Console.WriteLine($"Twoje saldo wynosi teraz: {hotel.Balance}");
                break;

            case 10:
                Console.WriteLine("---------TEST------------\n" +
                    "Testowanie funkcjonowania hotelu...\n\n" +
                    $"Rezerwacja pokoi: {Test.MakeReservationsTest()}\n" +
                    $"Anulowanie rezerwacji: {Test.CancelReservationsTest()}\n" +
                    $"Modyfikowanie rezerwacji: {Test.ModifyReservationsTest()}\n" +
                    $"Dodanie salda: {Test.AddBalance()}\n" +
                    $"Sprawdzanie pokoi: {Test.CheckARoomTest()}\n\n" +
                    $"Koniec testu...");
                break;

            default:
                ending = true;
                Console.WriteLine("Do zobaczenia!!!");
                break;
        }

        Console.WriteLine("\n\nWciśnij klawisz, aby kontynuować...");
        Console.ReadKey();
    }

    public bool StopMenu()
    {
        return ending;
    }
}

class Pokoj
{
    public int NrPokoju { get; }

    public int Price { get; } = 100;

    public enum RoomStatus
    {
        Free,
        Cleaning,
        Occupied
    }

    public RoomStatus RoomState { get; set; }

    public Pokoj(int nrPokoju, RoomStatus RoomState = RoomStatus.Free)
    {
        this.NrPokoju = nrPokoju;
        this.RoomState = RoomState;
    }

    public bool IsRoomFree()
    {
        return (RoomState == RoomStatus.Free);
    }

    public bool BookARoom(int balance)
    {
        if (RoomState == RoomStatus.Free && balance >= this.Price)
        {
            RoomState = RoomStatus.Occupied;
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool UnbookARoom()
    {
        if (RoomState == RoomStatus.Occupied)
        {
            RoomState = RoomStatus.Free;
            return true;
        }
        else
        {
            return false;
        }
    }
}

class PokojVIP : Pokoj
{
    new public int Price { get; } = 200;

    public PokojVIP(int nrPokoju, RoomStatus RoomState = RoomStatus.Free) : base(nrPokoju) { }
}

class Hotel
{
    private Pokoj[,] pokoje;
    private PokojVIP[] pokojeVIP;

    public enum HotelStatus
    {
        Open,
        Maintenance,
        Close
    }

    public int Balance { get; set; }

    public int Floor { get; }

    public int Rooms { get; }

    public HotelStatus HotelState { get; set; }

    public Hotel(int balance, int floor = 3, int rooms = 5, HotelStatus HotelState = HotelStatus.Open)
    {
        this.Balance = balance;
        this.Floor = floor;
        this.Rooms = rooms;
        this.pokoje = new Pokoj[floor - 1, rooms];
        this.pokojeVIP = new PokojVIP[rooms];
        this.HotelState = HotelState;
        int tempNumerPokoju;

        for (int i = 0; i < pokoje.Length; i++)
        {
            tempNumerPokoju = (i / Rooms + 1) * 100 + (i % Rooms + 1);
            pokoje[i / Rooms, i % Rooms] = new Pokoj(tempNumerPokoju);
        }
        for (int i = 0; i < pokojeVIP.Length; i++)
        {
            tempNumerPokoju = Floor * 100 + (i % Rooms + 1);
            pokojeVIP[i] = new PokojVIP(tempNumerPokoju);
        }
    }

    public Pokoj GetARoom(int tempNumerPokoju)
    {
        return pokoje[tempNumerPokoju / 100 - 1, tempNumerPokoju % 100 - 1];
    }

    public int SelectARoom()
    {
        if (HotelState != HotelStatus.Open)
        {
            Console.WriteLine("Niestety hotel jest teraz nieczynny.\n" +
                "Nie można teraz operować na pokojach.\n" +
                "Zapraszamy później!");
            return 0;
        }

        do
        {
            try
            {
                Console.WriteLine("Podaj numer pokoju: ");
                int tempNumerPokoju = Convert.ToInt32(Console.ReadLine());

                if (tempNumerPokoju > Floor * 100)
                {
                    PokojVIP wybrany = pokojeVIP[tempNumerPokoju % 100 - 1];
                }
                else
                {
                    Pokoj wybrany = pokoje[tempNumerPokoju / 100 - 1, tempNumerPokoju % 100 - 1];
                }

                return tempNumerPokoju;
            }
            catch
            {
                Console.WriteLine("\nWybrano zły numer pokoju!!!");
            }
        } while (true);
    }

    public void MakeReservation()
    {
        do
        {
            int tempNumerPokoju = SelectARoom();
            if (tempNumerPokoju < Floor * 100)
            {
                Pokoj wybrany = pokoje[tempNumerPokoju / 100 - 1, tempNumerPokoju % 100 - 1];

                if (Balance < wybrany.Price)
                {
                    Console.WriteLine("\nNiestety masz za mało pieniędzy :(");
                    break;
                }
                else if (wybrany.BookARoom(Balance))
                {
                    Balance -= wybrany.Price;
                    Console.WriteLine($"\nZarezerwowano pokój {wybrany.NrPokoju}");
                    break;
                }
                else
                {
                    Console.WriteLine("\nPokój jest już zarezerwowany!!!");
                }
            }
            else
            {
                PokojVIP wybrany = pokojeVIP[tempNumerPokoju % 100 - 1];

                if (Balance < wybrany.Price)
                {
                    Console.WriteLine("\nNiestety masz za mało pieniędzy :(");
                    break;
                }
                else if (wybrany.BookARoom(Balance))
                {
                    Balance -= wybrany.Price;
                    Console.WriteLine($"\nZarezerwowano pokój {wybrany.NrPokoju}");
                    break;
                }
                else
                {
                    Console.WriteLine("\nPokój jest już zarezerwowany!!!");
                }
            }
        } while (true);
    }

    public void MakeReservations()
    {
        int ilePokoi = 0;
        Console.WriteLine("Podaj ile rezerwacji chcesz zrobić lub anulować: ");
        do
        {
            try
            {
                ilePokoi = Convert.ToInt32(Console.ReadLine());
            }
            catch
            {
                Console.WriteLine("\nNie wpisano liczby!!! Sprobuj jeszcze raz: ");
                continue;
            }
            if (ilePokoi > TotalFreeRooms())
                Console.WriteLine("\nNiestety nie ma tyle wolnych pokoi!!! Podaj liczbę jeszcze raz: ");
            else if (ilePokoi <= 0)
                Console.WriteLine("\nLiczba mniejsza niż 1!!! Podaj liczbę jeszcze raz: ");
        } while (ilePokoi <= 0 || ilePokoi > TotalFreeRooms());

        for (int i = 0; i < ilePokoi; i++)
        {
            MakeReservation();
        }
    }

    public void CancelReservation()
    {
        do
        {
            int tempNumerPokoju = SelectARoom();
            if (tempNumerPokoju < Floor * 100)
            {
                Pokoj wybrany = pokoje[tempNumerPokoju / 100 - 1, tempNumerPokoju % 100 - 1];
                if (wybrany.UnbookARoom())
                {
                    Balance += wybrany.Price;
                    Console.WriteLine($"\nAnulowano rezerwacje pokoju {wybrany.NrPokoju}");
                    break;
                }
                else
                {
                    Console.WriteLine("\nPokój nie ma jeszcze rezerwacji!!!");
                }
            }
            else
            {
                PokojVIP wybrany = pokojeVIP[tempNumerPokoju % 100 - 1];
                if (wybrany.UnbookARoom())
                {
                    Balance += wybrany.Price;
                    Console.WriteLine($"\nAnulowano rezerwacje pokoju {wybrany.NrPokoju}");
                    break;
                }
                else
                {
                    Console.WriteLine("\nPokój nie ma jeszcze rezerwacji!!!");
                }
            }
        } while (true);
    }

    public void CancelReservations()
    {
        int ilePokoi = 0;
        int roomsOccupied = pokoje.Length + pokojeVIP.Length - TotalFreeRooms();
        Console.WriteLine("Podaj ile rezerwacji chcesz zrobić lub anulować: ");
        do
        {
            try
            {
                ilePokoi = Convert.ToInt32(Console.ReadLine());
            }
            catch
            {
                Console.WriteLine("\nNie wpisano liczby!!! Sprobuj jeszcze raz: ");
                continue;
            }
            if (ilePokoi > roomsOccupied)
                Console.WriteLine("\nNiestety nie ma tyle zajętych pokoi!!! Podaj liczbę jeszcze raz: ");
            else if (ilePokoi <= 0)
                Console.WriteLine("\nLiczba mniejsza niż 1!!! Podaj liczbę jeszcze raz: ");
        } while (ilePokoi <= 0 || ilePokoi > roomsOccupied);

        for (int i = 0; i < ilePokoi; i++)
        {
            CancelReservation();
        }
    }

    public void CheckARoom()
    {
        int tempNumerPokoju = SelectARoom();

        if (tempNumerPokoju < Floor * 100)
        {
            Pokoj wybrany = pokoje[tempNumerPokoju / 100 - 1, tempNumerPokoju % 100 - 1];
            if (wybrany.IsRoomFree())
            {
                Console.WriteLine($"\nPokój jest wolny!!! Jego cena to: {wybrany.Price}");
            }
            else
            {
                Console.WriteLine("\nPokój jest zajęty!!!");
            }
        }
        else
        {
            PokojVIP wybrany = pokojeVIP[tempNumerPokoju % 100 - 1];
            if (wybrany.IsRoomFree())
            {
                Console.WriteLine($"\nPokój jest wolny!!! Jego cena to: {wybrany.Price}");
            }
            else
            {
                Console.WriteLine("\nPokój jest zajęty!!!");
            }
        }
    }

    public int TotalFreeRooms()
    {
        int licznik = 0;
        for (int i = 0; i < Floor - 1; i++)
        {
            for (int j = 0; j < Rooms; j++)
            {
                if (pokoje[i, j].IsRoomFree()) licznik++;
            }
        }
        for (int j = 0; j < Rooms; j++)
        {
            if (pokojeVIP[j].IsRoomFree()) licznik++;
        }
        return licznik;
    }

    public override string ToString()
    {
        StringBuilder schemat = new StringBuilder(pokoje.Length);
        for (int i = 0; i < pokoje.Length; i++)
        {
            if (pokoje[i / Rooms, i % Rooms].IsRoomFree()) schemat.Append($"{pokoje[i / Rooms, i % Rooms].NrPokoju}-0 ");
            else schemat.Append($"{pokoje[i / Rooms, i % Rooms].NrPokoju}-X ");

            if ((i + 1) % Rooms == 0) schemat.Append("\n\n");
        }
        for (int i = 0; i < pokojeVIP.Length; i++)
        {
            if (pokojeVIP[i].IsRoomFree()) schemat.Append($"{pokojeVIP[i].NrPokoju}-0 ");
            else schemat.Append($"{pokojeVIP[i % Rooms].NrPokoju}-X ");

            if ((i + 1) % Rooms == 0) schemat.Append("\n\n");
        }
        return schemat.ToString();
    }
}

static class Test
{
    public static List<Pokoj> GenerateObject()
    {
        Hotel hotel = new Hotel(1000);
        List<Pokoj> pokoje = new List<Pokoj>();

        for (int i = 0; i < hotel.Rooms; i++)
        {
            pokoje.Add(hotel.GetARoom(100 + i + 1));
        }
        return pokoje;
    }

    public static bool MakeReservationsTest()
    {
        List<Pokoj> pokoje = GenerateObject();
        pokoje = GenerateObject();
        try
        {
            foreach (var pokoj in pokoje)
            {
                pokoj.BookARoom(1000);
            }
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool CancelReservationsTest()
    {
        List<Pokoj> pokoje = GenerateObject();
        try
        {
            foreach (var pokoj in pokoje)
            {
                pokoj.UnbookARoom();
            }
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool AddBalance()
    {
        Hotel hotel = new Hotel(10);
        try
        {
            hotel.Balance += 9999;
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool ModifyReservationsTest()
    {
        List<Pokoj> pokoje = GenerateObject();
        try
        {
            pokoje[0].BookARoom(1000);
            pokoje[0].UnbookARoom();
            pokoje[1].BookARoom(1000);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool CheckARoomTest()
    {
        List<Pokoj> pokoje = GenerateObject();
        try
        {
            foreach (var pokoj in pokoje)
            {
                pokoj.IsRoomFree();
            }
            return true;
        }
        catch
        {
            return false;
        }
    }
}