using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class UglyKeepName : MonoBehaviour
{
    static string playerName = string.Empty;
    InputField inputField;

    private List<string> animals = new List<string>()
    {
        "Aardvark",
        "Anteater",
        "Antelope",
        "Armadillo",
        "Aye Aye",
        "Baboon",
        "Badger",
        "Bandicoot",
        "Bat",
        "Bear",
        "Beaver",
        "Bengal Tiger",
        "Binturong",
        "Bison",
        "Black Bear",
        "Blue Whale",
        "Bobcat",
        "Bongo",
        "Bonobo",
        "Brown Bear",
        "Buffalo",
        "Camel",
        "Capybara",
        "Caracal",
        "Cat",
        "Chamois",
        "Cheetah",
        "Chimpanzee",
        "Chinchilla",
        "Chipmunk",
        "Coati",
        "Cougar",
        "Cow",
        "Coyote",
        "Cuscus",
        "Deer",
        "Dhole",
        "Dingo",
        "Dog",
        "Dolphin",
        "Donkey",
        "Dormouse",
        "Dugong",
        "Echidna",
        "Elephant",
        "Fennec Fox",
        "Ferret",
        "Fossa",
        "Fox",
        "Fur Seal",
        "Gerbil",
        "Gibbon",
        "Giraffe",
        "Goat",
        "Gopher",
        "Gorilla",
        "Grey Seal",
        "Grizzly Bear",
        "Guinea Pig",
        "Hamster",
        "Hare",
        "Hedgehog",
        "Hippopotamus",
        "Horse",
        "Howler Monkey",
        "Human",
        "Humpback Whale",
        "Hyena",
        "Impala",
        "Indri",
        "Jackal",
        "Jaguar",
        "Kangaroo",
        "Killer Whale",
        "Koala",
        "Kudu",
        "Lemming",
        "Lemur",
        "Leopard",
        "Liger",
        "Lion",
        "Llama",
        "Lynx",
        "Manatee",
        "Mandrill",
        "Markhor",
        "Meerkat",
        "Minke Whale",
        "Mole",
        "Mongoose",
        "Monkey",
        "Moose",
        "Mouse",
        "Mule",
        "Numbat",
        "Ocelot",
        "Okapi",
        "Opossum",
        "Orang-utan",
        "Otter",
        "Pademelon",
        "Panther",
        "Pig",
        "Pika",
        "Platypus",
        "Polar Bear",
        "Porcupine",
        "Possum",
        "Puma",
        "Quokka",
        "Quoll",
        "Rabbit",
        "Raccoon",
        "Rat",
        "Red Panda",
        "Red Wolf",
        "Reindeer",
        "Rhinoceros",
        "Rock Hyrax",
        "Saola",
        "Sea Lion",
        "Sea Otter",
        "Seal",
        "Serval",
        "Sheep",
        "Skunk",
        "Sloth",
        "Sperm Whale",
        "Spider Monkey",
        "Squirrel",
        "Stoat",
        "Sun Bear",
        "Tapir",
        "Tarsier",
        "Tasmanian Devil",
        "Tiger",
        "Uakari",
        "Vampire Bat",
        "Wallaby",
        "Walrus",
        "Warthog",
        "Water Buffalo",
        "Water Vole",
        "Weasel",
        "White Tiger",
        "Wild Boar",
        "Wildebeest",
        "Wolf",
        "Wolverine",
        "Wombat",
        "Yak",
        "Zebra",
        "Zebu",
        "Zonkey",
        "Zorse",
    };
    private List<string> adjectives = new List<string>()
    {
        "Attractive",
        "Bald",
        "Beautiful",
        "Chubby",
        "Clean",
        "Dazzling",
        "Drab",
        "Elegant",
        "Fancy",
        "Fit",
        "Flabby",
        "Glamorous",
        "Gorgeous",
        "Handsome",
        "Magnificent",
        "Muscular",
        "Plain",
        "Plump",
        "Scruffy",
        "Shapely",
        "Skinny",
        "Stocky",
        "Unkempt",
        "Unsightly",
        "Agreeable",
        "Ambitious",
        "Brave",
        "Calm",
        "Delightful",
        "Eager",
        "Faithful",
        "Gentle",
        "Happy",
        "Jolly",
        "Kind",
        "Lively",
        "Nice",
        "Obedient",
        "Polite",
        "Proud",
        "Silly",
        "Thankful",
        "Victorious",
        "Witty",
        "Wonderful",
        "Zealous",
    };

    void Start()
    {
        inputField = GetComponent<InputField>();
        if (string.IsNullOrEmpty(playerName) == false)
        {
            inputField.text = playerName;
        }
        else
        {
            //adjectives = readFile("Assets/Adjectives.txt");
            //animals = readFile("Assets/Animals.txt");

            string name = adjectives[Random.Range(0, adjectives.Count)] + " " + animals[Random.Range(0, animals.Count)];

            while(name.Length > 16)
                name = adjectives[Random.Range(0, adjectives.Count)] + " " + animals[Random.Range(0, animals.Count)];
            inputField.text = name;
        }
    }

    void Update()
    {
        playerName = inputField.text;
    }

    List<string> readFile(string path)
    {
        List<string> list = new List<string>();

        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader(path);

        while (!reader.EndOfStream)
        {
            list.Add(reader.ReadLine());
        }

        Debug.Log(animals.Count);

        reader.Close();


        return list;
    }
}
