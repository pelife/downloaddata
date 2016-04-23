CREATE TABLE "tb_valor_titulo_publico_bcb" (
	`dt_valor`	INTEGER NOT NULL,
	`sg_titulo`	TEXT NOT NULL,
	`tp_titulo`	TEXT NOT NULL,
	`dt_vencimento`	INTEGER,
	`vl_taxa_compra`	REAL NOT NULL,
	`vl_taxa_venda`	REAL NOT NULL,
	`vl_pu_compra`	REAL NOT NULL,
	`vl_pu_venda`	REAL NOT NULL,
	`vl_pu_base`	REAL,
	PRIMARY KEY(dt_valor,sg_titulo)
)