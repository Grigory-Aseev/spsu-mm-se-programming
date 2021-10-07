#include <stdio.h>

void is_prime(int a, int b, int c)
{
	int max = 0;
	if (a > b && a > c)
	{
		max = a;
	}
	else if (b > c)
	{
		max = b;
	}
	else
	{
		max = c;
	}
	for (int i = max; i > 0; i--)
	{
		if (a % i == 0 && b % i == 0 && c % i == 0)
		{
			if (i == 1)
			{
				printf("Inputted numbers are prime Pythagorean triples.");
			}
			else 
			{
				printf("Inputted numbers are Pythagorean triples.");
				break;
			}	
		}
	}
}

void clear_buffer()
{
	while (getchar() != '\n');
}

int is_pythagorean_triple(int a, int b, int c)
{
	return (a * a + b * b == c * c) || (a * a + c * c == b * b) || (b * b + c * c == a * a);
}

int main()
{
	int a, b, c, correctly_read;
	printf("This program checks that inputted triples are Pythagorean and prime.\n");
	do 
	{
		printf("Enter three natural numbers satisfying the Pythagorean formula x ^ 2 + y ^ 2 = z ^ 2 : ");
		correctly_read = scanf_s("%u%u%u", &a, &b, &c);
		clear_buffer(stdin);
	} 
	while (correctly_read != 3 || a <= 0 || b <= 0 || c <= 0);
	if (is_pythagorean_triple(a, b, c))
	{
		is_prime(a, b, c);
	}
	else
	{
		printf("Inputted numbers are not Pythagorean triples.");
	}
	return 0;
}
